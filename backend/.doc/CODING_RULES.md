# CODING_RULES - 编码规范

> **核心原则：严格遵守 SOLID、KISS、DRY 原则。**
>
> - **SOLID**：单一职责、开闭原则、里氏替换、接口隔离、依赖反转
> - **KISS**：保持简单，避免过度设计
> - **DRY**：不要重复自己，抽象共用逻辑
>
> **编码红线和规范。违反红线的代码不会被接受。**

---

## ⚠️ 必读：文档权限层级

**在进行任何开发前，必须理解以下文档层级：**

### 宪法级文档（绝对只读）

| 路径 | 权限 | 说明 |
|------|------|------|
| `.raw_materials/` | 🔴 只读 | 项目的最高权威文档，AI 只能读取，不能写入 |
| `.raw_materials/BUSINESS_RULES/` | 🔴 绝对只读 | 费率表、API 契约，任何修改都是违规 |
| `.raw_materials/TECH_CONSTRAINTS/` | 🟡 只读可异议 | 技术约束，有问题可提出但不能擅自修改 |
| `**/README.md` | 🔴 只读 | 人写给人看的文档，AI 不能修改/删除/创建 |

### 工作文档（可读写）

| 路径 | 权限 | 说明 |
|------|------|------|
| `.doc/` | ✅ 可读写 | AI 的工作文档，自行维护 |
| `src/` 代码目录 | ✅ 可读写 | 按规范编写代码（除红线文件外） |
| `**/AI_GUIDE.md` | ✅ 可读写 | AI 的导航文档，每完成一个 Issue 需更新 |

### 异议机制

如果发现 `.raw_materials/` 或 `README.md` 中的内容有问题：

```markdown
> **[异议]**
> - 文档说：XXX
> - 实际情况：YYY
> - 我的判断：ZZZ
> - 建议：等待人类确认
```

**宁可停下来问，也不要擅自修改只读文档。**

---

## 1. 禁止修改的文件（绝对红线）

| 文件/目录 | 原因 |
|-----------|------|
| `FairWorkly.Domain/*/Entities/*.cs` | Entity 由 Team Lead 设计，已定稿 |
| `FairWorkly.Infrastructure/Persistence/FairWorklyDbContext.cs` | SaveChangesAsync 审计逻辑已配置完成 |

---

## 2. 架构约束

### 2.1 分层架构

```
┌─────────────────────────────────────┐
│           FairWorkly.API            │  ← Controller（只做转发）
├─────────────────────────────────────┤
│       FairWorkly.Application        │  ← 用例、服务、编排
├─────────────────────────────────────┤
│         FairWorkly.Domain           │  ← Entity、枚举（不可改）
├─────────────────────────────────────┤
│      FairWorkly.Infrastructure      │  ← 数据库、外部服务
└─────────────────────────────────────┘
```

### 2.2 Feature 目录结构（CQRS + Vertical Slicing）

```
FairWorkly.Application/Payroll/Features/
└── ValidatePayroll/
    ├── ValidatePayrollCommand.cs      # IRequest<TResponse>
    ├── ValidatePayrollValidator.cs    # AbstractValidator<TCommand>
    ├── ValidatePayrollHandler.cs      # IRequestHandler<TCommand, TResponse>
    └── ValidationResultDto.cs         # 响应 DTO
```

### 2.3 Service 目录结构

```
FairWorkly.Application/Payroll/
├── Features/
│   └── ValidatePayroll/
├── Services/
│   ├── CsvParserService.cs
│   ├── EmployeeSyncService.cs
│   └── ComplianceEngine/
│       ├── IComplianceRule.cs
│       ├── BaseRateRule.cs
│       └── ...
└── Interfaces/
    ├── ICsvParserService.cs
    └── IEmployeeSyncService.cs
```

### 2.4 Repository 位置

```
# 接口 (Application 层)
FairWorkly.Application/Payroll/Interfaces/IPayslipRepository.cs

# 实现 (Infrastructure 层)
FairWorkly.Infrastructure/Persistence/Repositories/Payroll/PayslipRepository.cs
```

### 2.5 Handler vs Orchestrator 职责划分

> ⚠️ **架构级约束**：详见 [ARCHITECTURE.md](../.raw_materials/TECH_CONSTRAINTS/ARCHITECTURE.md)

**Handler（业务流程的"总指挥"）**：
- 数据校验（Pre-Validation）
- 业务逻辑判断（if-else 分支决策）
- 调用各种 Service 和 Repository
- 调用 Orchestrator（如果需要 AI）
- 组装最终返回结果

**Orchestrator（AI 技能的封装）**：
- 组装发送给 AI 的 Payload
- 调用 Python HTTP 接口
- 解析 AI 返回的响应
- **不包含业务逻辑，不做流程判断**

**什么时候需要 Orchestrator？**

| 场景 | 需要 AI 调用？ | 需要 Orchestrator？ |
|------|---------------|---------------------|
| 智能问答（RAG） | ✅ | ✅ |
| 排班风险分析（AI 推理） | ✅ | ✅ |
| 薪资合规检查（纯规则计算） | ❌ | ❌ |
| 员工 CRUD | ❌ | ❌ |

**禁止事项**：
- ❌ 在 Orchestrator 中写业务逻辑
- ❌ 在 Orchestrator 中做数据校验
- ❌ 为不需要 AI 的模块创建 Orchestrator
- ❌ 在 Controller 中编排业务流程

---

## 3. 代码规范

### 3.1 数据类型

| 场景 | 必须使用 | 禁止使用 |
|------|----------|----------|
| 金额字段 | `decimal` | `float`, `double` |
| 时间戳 | `DateTimeOffset` | `DateTime` |
| 日期（无时间） | `DateOnly` | `DateTime` |

### 3.2 获取当前时间

```csharp
// ✅ 正确：注入 IDateTimeProvider
public class MyService
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public MyService(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public void DoSomething()
    {
        var now = _dateTimeProvider.UtcNow;  // ✅
    }
}

// ❌ 禁止：直接调用静态方法
var now = DateTime.Now;           // ❌
var now = DateTime.UtcNow;        // ❌
var now = DateTimeOffset.Now;     // ❌
var now = DateTimeOffset.UtcNow;  // ❌
```

### 3.3 依赖注入注册

**必须在对应层的 `DependencyInjection.cs` 中注册，禁止在 `Program.cs` 中直接注册。**

```csharp
// ✅ Application 层服务 → Application/DependencyInjection.cs
services.AddScoped<ICsvParserService, CsvParserService>();

// ✅ Infrastructure 层服务 → Infrastructure/DependencyInjection.cs
services.AddScoped<IPayslipRepository, PayslipRepository>();

// ❌ 禁止在 Program.cs 中直接注册业务服务
```

### 3.4 数值精度与容差

```csharp
// 时薪比对容差：$0.01
const decimal RateTolerance = 0.01m;

// 罚金/养老金比对容差：$0.05
const decimal PayTolerance = 0.05m;

// 比对逻辑
if (actualRate < expectedRate - RateTolerance)
{
    // 违规
}
```

### 3.5 语言规范

| 场景 | 语言 |
|------|------|
| 代码注释 | English |
| 变量/方法命名 | English |
| Git Commit Message | English |
| .doc/ 文档 | 中文或英文均可 |

---

## 4. 常见错误

### ❌ 错误 1：在 Handler 中写验证逻辑

```csharp
// ❌ 错误
public async Task<Result> Handle(Command request, CancellationToken ct)
{
    if (string.IsNullOrEmpty(request.Title))  // ❌ 验证应在 Validator 中
        throw new ValidationException("Title is required");
}

// ✅ 正确：创建单独的 Validator
public class CommandValidator : AbstractValidator<Command>
{
    public CommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
    }
}
```

### ❌ 错误 2：业务逻辑写在 Controller 中

```csharp
// ❌ 错误
[HttpPost]
public async Task<IActionResult> Validate(Request request)
{
    var employee = await _repository.GetByIdAsync(request.EmployeeId);  // ❌
    if (employee.Rate < MinimumRate) { ... }  // ❌ 业务逻辑
}

// ✅ 正确：Controller 只做转发
[HttpPost]
public async Task<IActionResult> Validate(Request request)
{
    var result = await _mediator.Send(new ValidateCommand { ... });
    return Ok(result);
}
```

### ❌ 错误 3：修改 Entity 添加业务逻辑

```csharp
// ❌ 错误：不要在 Entity 中添加方法
public class Payslip : AuditableEntity
{
    public bool IsCompliant()  // ❌ 业务逻辑应在 Service/Rule 中
    {
        return this.HourlyRate >= MinimumRate;
    }
}
```

---

## 5. 测试要求

### 5.1 测试时机

- 不强制 TDD
- 每个 Issue 完成后，必须编写对应的测试
- 测试通过后才能进入下一个 Issue

### 5.2 测试文件位置

```
FairWorkly.UnitTests/
├── Unit/                           # 单元测试 (用 InMemory)
│   ├── CsvParserServiceTests.cs
│   ├── EmployeeSyncServiceTests.cs
│   ├── BaseRateRuleTests.cs
│   └── ...
├── Integration/                    # 集成测试 (用本地 PostgreSQL)
│   └── PayrollValidationTests.cs
└── TestData/Csv/                   # 测试数据（已存在）
    └── ...
```

### 5.3 数据库操作

```bash
# 清库重建（集成测试前执行）
dotnet ef database drop --force --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API
dotnet ef database update --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API
```

### 5.4 AI Agent 数据库权限

**✅ 已授权：测试阶段可随时清库重建（drop + migrate）**

- 测试数据可自行决定是否保留
- 以方便测试为原则，无需事先询问
- 此为持久性授权（2025-12-28）

---

## 6. AI_GUIDE 更新要求

**每完成一个 Issue，必须检查并更新相关的 AI_GUIDE.md。**

检查清单：
- [ ] 新建的目录是否需要 AI_GUIDE？
- [ ] 已有的 AI_GUIDE 内容是否需要更新？
- [ ] 复杂逻辑是否需要单独的说明？

---

## 7. AI Agent 行为规范

### 7.1 获取当前日期

Claude Code 通过系统上下文中的 `Today's date` 字段获取当前日期，**不是**通过内部训练数据推断。

在 DEVLOG.md 或其他需要记录日期的地方，使用系统提供的日期，格式为 `YYYY-MM-DD`。

### 7.2 时间相关注意事项

- 系统上下文中的日期是可信的
- 如果需要记录时间戳，应询问人类或使用系统提供的日期
- 不要凭"感觉"或"记忆"推断日期

---

## 8. AI Commit 规则

AI Agent 的 commit 权限由各 Issue 文档单独授权。

**通用规则**：

| 规则 | 要求 |
|------|------|
| **语言** | Commit message 必须 **全英文**（标题、正文均不可出现中文） |
| **格式** | Conventional Commits (`feat:`, `test:`, `fix:`, `chore:`, `docs:`) |
| **粒度** | 按逻辑单元提交（一个功能点 = 代码 + 测试） |
| **测试** | 提交前必须运行 `dotnet test` 确保通过 |
| **Push** | ❌ 禁止 push 到远程，仅 commit 到本地 |
| **签名** | ❌ 禁止添加 AI 生成标识（如 "Generated with Claude Code"、Co-Authored-By 等） |

**权限来源**：

具体的 commit 权限范围（可提交哪些文件）由各 Issue 文档定义，例如：
- `ISSUE_02_ComplianceEngine.md` 中的 "AI Commit 权限" 章节

**无授权 = 无权限**：

如果当前 Issue 文档中没有 "AI Commit 权限" 章节，则 AI Agent 不可自动提交代码。
