# FairBot API Security Fix: Agent Service 认证保护

**状态**：已完成
**完成日期**：2026-02-23
**分支**：`Lillian-239-refactor-fairbot-from-chat-first-to-action-first-landing-page`

---

## 问题概述

**原始状态**：前端 FairBot 页面直接请求 Agent Service (`http://localhost:8000`)，绕过了 .NET Backend 的 JWT 认证体系。Agent Service 的 `/api/agent/chat` 端点对外完全开放，任何人都可以无需登录直接调用。

**影响范围**：Agent Service 的所有端点（compliance Q&A、roster explain、roster parse、payroll verify）

**严重程度**：P0

---

## 原始架构 vs 修复后架构

### 原始架构（有安全缺陷）

```
Browser ──── POST http://agent-service:8000/api/agent/chat ──→ Agent Service
              （直接请求，无 JWT，无认证）
```

前端代码 `fairbotApi.ts` 创建了一个独立的 axios 实例，直连 Agent Service：

```typescript
// 修复前：frontend/src/services/fairbotApi.ts
const agentBaseURL = import.meta.env.VITE_AGENT_SERVICE_URL ?? 'http://localhost:8000'

const agentClient = axios.create({
  baseURL: agentBaseURL,  // 直接指向 Agent Service
  timeout: 30000,
})
```

同时 Agent Service 的 CORS 配置允许任何来源：

```python
# 修复前：agent-service/master_agent/main.py
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],   # 允许所有来源
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)
```

### 修复后架构（安全）

```
Browser ── JWT ──→ .NET Backend /api/fairbot/chat ── 内网调用 ──→ Agent Service
                   （验证身份 + 限流 + 审计）                （不暴露给外部）
```

---

## 为什么必须修复

### 1. API 端点无认证 = 任何人都能调用

虽然前端有 `ProtectedRoute` + `RoleBasedRoute(admin)` 保护页面访问，但这只是**前端路由守卫**，阻止的是"用户看到页面"，不是"API 被调用"。

任何人只要知道 Agent Service 的地址，就能直接调用：

```bash
# 无需任何 token，直接调用 LLM
curl -X POST http://your-domain:8000/api/agent/chat \
  -F "message=explain penalty rates for retail workers"
```

**类比**：前端路由守卫 = 锁了前门，Agent Service 无认证 = 后窗大开。

### 2. LLM 调用有实际成本

每次请求都会触发 OpenAI/Anthropic API 调用，产生 token 费用。无限流控制意味着：
- 恶意用户可以批量请求，快速消耗 API 额度
- 没有用户级别的用量追踪

### 3. 部署时 Agent Service 端口必须暴露

当前架构要求 Agent Service 的 8000 端口对浏览器可达。在生产环境中，这意味着要么：
- 直接暴露端口（安全风险）
- 通过反向代理暴露（增加复杂性，且仍无认证）

### 4. 与现有安全架构不一致

项目中**所有其他功能**（roster upload、compliance check、settings）都通过 .NET Backend 路由，受 JWT 保护。FairBot 是唯一的例外。

---

## 完成的修复工作

### 修复 1: Backend 代理端点

在 .NET Backend 新增 `FairBotController`，接收前端请求，验证 JWT 后转发给 Agent Service。

**新增文件**：

- `backend/src/FairWorkly.API/Controllers/FairBot/FairBotController.cs`
- `backend/src/FairWorkly.Application/FairBot/FairBotChatResponse.cs`（Agent Service 响应 DTO）

**IAiClient 接口新增方法**：

```csharp
// backend/src/FairWorkly.Application/Common/Interfaces/IAiClient.cs
Task<TResponse> PostFormAsync<TResponse>(
    string route,
    Dictionary<string, string> formFields,
    CancellationToken cancellationToken = default
);
```

**Controller 实现**：

```csharp
[Route("api/fairbot")]
[Authorize(Policy = "RequireAdmin")]  // 仅 Admin 可调用
public class FairBotController(IAiClient aiClient, ICurrentUserService currentUser) : BaseApiController
{
    [HttpPost("chat")]
    public async Task<IActionResult> Chat(
        [FromForm] string message,
        [FromForm] string? intentHint = null,
        [FromForm] string? contextPayload = null,
        CancellationToken cancellationToken = default)
    {
        // JWT 已由 [Authorize] 验证
        // 构建 form fields，通过 PythonAiClient 转发到 Agent Service
    }
}
```

### 修复 2: 前端改用 httpClient

将请求从直连 Agent Service 改为通过 .NET Backend，复用已有的 `httpClient`（自动带 JWT）。

**修改文件**：`frontend/src/services/fairbotApi.ts`

```typescript
// 修复后：通过 .NET Backend，复用 httpClient（自带 JWT interceptor）
import httpClient from './httpClient'

export async function sendChatMessage(message: string, options?: SendChatMessageOptions): Promise<AgentChatResponse> {
  const formData = new FormData()
  formData.append('message', message)
  // ...
  const response = await httpClient.post<AgentChatResponse>('/fairbot/chat', formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
    timeout: 30000,  // LLM 调用需要更长超时
  })
  return response.data
}
```

同时移除了 `frontend/.env.example` 中的 `VITE_AGENT_SERVICE_URL`，前端不再需要知道 Agent Service 地址。

### 修复 3: 收紧 Agent Service CORS

**修改文件**：`agent-service/master_agent/main.py`

```python
# 修复后：从环境变量读取允许的来源，默认仅允许 Backend
allowed_origins = os.getenv("ALLOWED_ORIGINS", "http://localhost:5680").split(",")

app.add_middleware(
    CORSMiddleware,
    allow_origins=allowed_origins,
    allow_credentials=False,
    allow_methods=["POST", "GET"],
    allow_headers=["*"],
)
```

### 修复 4: .NET 8 JWT Role Claim 映射问题

**实施过程中发现的问题**：`[Authorize(Policy = "RequireAdmin")]` 始终返回 403 Forbidden，即使 JWT 中确实包含 `"role": "Admin"`。

**根因**：.NET 8 默认使用 `JsonWebTokenHandler`，它会自动重映射 JWT claim 名称。我们的 JWT 中 role 存储为 `"role": "Admin"`，但 `RequireRole("Admin")` 默认查找的是 `ClaimTypes.Role`（`http://schemas.microsoft.com/ws/2008/06/identity/claims/role`），导致匹配失败。

**之前没有暴露**的原因：项目中所有 Controller 只用了 `[Authorize]`（仅验证 JWT 有效性），从未用过 `[Authorize(Policy = "RequireAdmin")]`（需要匹配 role claim），所以这个问题一直隐藏着。

**修复方式**：在 `backend/src/FairWorkly.API/Program.cs` 的 JWT 配置中添加：

```csharp
options.MapInboundClaims = false; // 禁止 .NET 重映射 JWT claim 名称

options.TokenValidationParameters = new TokenValidationParameters
{
    // ...其他配置...
    RoleClaimType = "role", // 告诉 .NET 使用我们 JWT 中的 "role" claim
};
```

**影响**：这个修复不仅对 FairBot 有效，也为未来所有使用 `RequireAdmin` / `RequireManager` 策略的 Controller 奠定了基础（例如 RosterController 未来加 policy 时就能直接生效）。

---

## 角色权限模型

| 角色 | 访问范围 | 说明 |
|------|---------|------|
| **Admin** | 全系统访问 | 包括 FairBot、Roster 合规检查、Settings 等所有功能 |
| **Manager** | Roster 合规检查 | 登录后仅能使用 Roster 上传和合规检查功能 |

因此：
- `FairBotController` 使用 `[Authorize(Policy = "RequireAdmin")]` — FairBot 仅限 Admin
- `RosterController` 未来应使用 `[Authorize]` 或更细粒度的策略 — Admin 和 Manager 都可访问

---

### 修复 5: 清理死代码 — 删除 MockAiClient

**删除的文件**：

- `backend/src/FairWorkly.Infrastructure/AI/Mocks/MockAiClient.cs`
- `backend/src/FairWorkly.Infrastructure/AI/Mocks/MockAiRouter.cs`
- `backend/src/FairWorkly.Infrastructure/AI/Mocks/Agents/ComplianceMock.cs`
- `backend/src/FairWorkly.Infrastructure/AI/Mocks/Agents/DocumentMock.cs`
- `backend/src/FairWorkly.Infrastructure/AI/Mocks/Agents/EmployeeMock.cs`
- `backend/src/FairWorkly.Infrastructure/AI/Mocks/Agents/PayrollMock.cs`

**修改的文件**：

- `backend/src/FairWorkly.Infrastructure/DependencyInjection.cs` — 移除 `UseMockAi` 条件分支，始终注册 `PythonAiClient`
- 5 个 `appsettings*.json` 文件 — 移除 `"UseMockAi"` 配置项
- `backend/README.md` — 更新 AI 服务章节，移除 Mock Mode 文档

**原因**：`MockAiClient` 的所有方法都只是 `throw new NotImplementedException()`，没有提供任何 mock 数据，是完全的死代码。

---

## 修复后的完整数据流

```
1. 用户在 FairBot 页面输入问题
         │
2. Frontend 调用 httpClient.post('/fairbot/chat', formData)
   （httpClient 自动附加 JWT Bearer token）
         │
3. .NET Backend 收到请求
   ├── [Authorize(Policy = "RequireAdmin")] 验证 JWT + role
   ├── ICurrentUserService 提取 userId / orgId
   ├── （可选）记录审计日志
   └── （可选）检查 rate limit
         │
4. Backend 通过 PythonAiClient.PostFormAsync() 转发到 Agent Service（内网调用）
         │
5. Agent Service 处理请求（intent routing → feature → LLM）
         │
6. 响应原路返回：Agent Service → Backend → Frontend
```

---

## 修改文件汇总

| 文件 | 变更类型 | 说明 |
|------|---------|------|
| `backend/src/FairWorkly.Application/Common/Interfaces/IAiClient.cs` | 修改 | 新增 `PostFormAsync` 方法 |
| `backend/src/FairWorkly.Infrastructure/AI/PythonServices/PythonAiClient.cs` | 修改 | 实现 `PostFormAsync` |
| `backend/src/FairWorkly.Application/FairBot/FairBotChatResponse.cs` | 新增 | Agent Service 响应 DTO |
| `backend/src/FairWorkly.API/Controllers/FairBot/FairBotController.cs` | 新增 | 代理端点 |
| `backend/src/FairWorkly.API/Program.cs` | 修改 | JWT claim 映射修复 |
| `backend/src/FairWorkly.Infrastructure/DependencyInjection.cs` | 修改 | 移除 mock 分支 |
| `backend/src/FairWorkly.Infrastructure/AI/Mocks/` | 删除 | 6 个死代码文件 |
| `backend/appsettings*.json` (5 个文件) | 修改 | 移除 `UseMockAi` |
| `backend/README.md` | 修改 | 更新文档 |
| `frontend/src/services/fairbotApi.ts` | 修改 | 改用 httpClient |
| `frontend/.env.example` | 修改 | 移除 `VITE_AGENT_SERVICE_URL` |
| `agent-service/master_agent/main.py` | 修改 | 收紧 CORS |

---

## 验证结果

### 自动化测试

- Backend 单元测试：190 个全部通过
- Backend 集成测试：41 个全部通过
- Frontend TypeScript 编译：无错误

### 手动端到端测试

```bash
# 测试 1: 无 JWT 调用 → 401 Unauthorized（正确拒绝）
curl -s -o /dev/null -w "%{http_code}" -X POST http://localhost:5680/api/fairbot/chat \
  -F "message=hello"
# 结果: 401

# 测试 2: 带 Admin JWT 调用 → 200 OK（正确通过）
TOKEN=$(curl -s -X POST http://localhost:5680/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@fairworkly.com.au","password":"fairworkly123"}' | jq -r '.data.accessToken')

curl -s -w "\n%{http_code}" -X POST http://localhost:5680/api/fairbot/chat \
  -H "Authorization: Bearer $TOKEN" \
  -F "message=hello"
# 结果: 200，返回 Agent Service 的正常响应
```
