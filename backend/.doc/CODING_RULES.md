# CODING_RULES - 编码规范

> **编码红线和规范。违反红线的代码不会被接受。**

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
