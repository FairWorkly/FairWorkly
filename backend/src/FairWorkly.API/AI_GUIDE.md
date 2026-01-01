# API Layer AI_GUIDE

> **API 层导航。包含 Controller 和 HTTP 端点配置。**

> ⚠️ **宪法文档提醒**：API 契约定义在 [.raw_materials/BUSINESS_RULES/API_Contract.md](../../.raw_materials/BUSINESS_RULES/API_Contract.md)，这是只读的。
> 响应结构必须符合契约，不能擅自修改。

---

## 概述

API 层是表现层，只负责：
1. 接收 HTTP 请求
2. 转发到 MediatR Handler
3. 返回响应

**Controller 不包含业务逻辑**，所有逻辑在 Application 层处理。

---

## 目录结构

```
FairWorkly.API/
├── Controllers/
│   └── Payroll/
│       └── PayrollController.cs     ⚠️ 骨架，待 ISSUE_03 实现
├── ExceptionHandlers/               ← 全局异常处理
├── Config/                          ← 配置类
├── Program.cs                       ← 应用入口
├── appsettings.json                 ← 配置文件
└── AI_GUIDE.md                      ← 本文件
```

---

## 开发状态

| 组件 | 状态 | 说明 |
|------|------|------|
| PayrollController | ⚠️ 骨架 | 待 ISSUE_03 实现 POST /api/payroll/validation |
| ExceptionHandlers | ✅ 已实现 | 全局异常处理 |
| Program.cs | ✅ 已配置 | MediatR, Swagger, CORS 等 |

---

## API 端点规划

### POST /api/payroll/validation

**状态**: ⏳ ISSUE_03 待实现

**请求**:
```
Content-Type: multipart/form-data

- file: CSV 文件 (必填)
- awardType: string (必填)
- payPeriod: string (必填)
- weekStarting: YYYY-MM-DD (必填)
- weekEnding: YYYY-MM-DD (必填)
- state: string (必填)
- enableBaseRateCheck: boolean (默认 true)
- enablePenaltyCheck: boolean (默认 true)
- enableCasualLoadingCheck: boolean (默认 true)
- enableSuperCheck: boolean (默认 true)
```

**响应**: 详见 [API_Contract.md](../../.raw_materials/BUSINESS_RULES/API_Contract.md)

---

## Controller 编写规范

```csharp
// ✅ 正确：Controller 只做转发
[ApiController]
[Route("api/payroll")]
public class PayrollController : ControllerBase
{
    private readonly IMediator _mediator;

    [HttpPost("validation")]
    public async Task<IActionResult> Validate([FromForm] ValidatePayrollCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new { code = 200, msg = "Success", data = result });
    }
}

// ❌ 错误：不要在 Controller 中写业务逻辑
```

---

## 相关文档

### 上游（调用方）
- 前端通过 HTTP 调用此层

### 下游（被调用方）
- [Application/Payroll](../FairWorkly.Application/Payroll/AI_GUIDE.md) - 业务逻辑处理

### 规范文档
- [/.doc/CODING_RULES.md](../../.doc/CODING_RULES.md) - 编码规范
- [/.raw_materials/BUSINESS_RULES/API_Contract.md](../../.raw_materials/BUSINESS_RULES/API_Contract.md) - API 契约（只读）

---

## 配置说明

### appsettings.json 关键配置

| 配置项 | 说明 |
|--------|------|
| `ConnectionStrings:DefaultConnection` | PostgreSQL 连接串 |
| `AiSettings:UseMockAi` | Mock AI 开关 (开发阶段 true) |
| `FileLogging:Enabled` | 文件日志开关 |

### Swagger

开发环境启动后访问：`https://localhost:5001/swagger`

---

## 文档矩阵链接

### 上级导航
- [← 返回仓库级 AI_GUIDE](../../AI_GUIDE.md)
- [← .doc/AI_GUIDE.md](../../.doc/AI_GUIDE.md) - 项目状态

### 依赖的下游服务
- [Application 层](../FairWorkly.Application/AI_GUIDE.md)
- [Payroll 模块](../FairWorkly.Application/Payroll/AI_GUIDE.md) ← **当前开发重点**

### 同级导航
- [Infrastructure 层](../FairWorkly.Infrastructure/AI_GUIDE.md)
- [Tests](../../tests/FairWorkly.UnitTests/AI_GUIDE.md)

### Issue 文档
- [ISSUE_03 (待开发)](../../.doc/issues/ISSUE_03_Handler_API.md) ← API 端点实现

---

*最后更新: 2026-01-01*
