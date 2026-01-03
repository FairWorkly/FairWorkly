# ISSUE_03: Handler 集成 + API 暴露

## 目标

集成所有组件，实现完整的验证流程并暴露 API 接口。

**一句话**：把 CSV 解析、员工同步、合规检查串联起来，通过 API 返回完整的审计报告。

---

## 状态

- [ ] 开发中
- [ ] 测试通过
- [ ] Review 完成

**前置依赖**:
- ISSUE_01 ✅ 已完成
- ISSUE_02 ✅ 已完成

---

## 输入与输出

- **输入**：CSV 文件 + 配置参数（AwardType, PayPeriod, State 等）
- **输出**：ValidationResultDto（符合 API 契约的审计报告）

---

## 交付物

### 新建文件

```
src/FairWorkly.Application/Payroll/Features/ValidatePayroll/
├── ValidatePayrollCommand.cs
├── ValidatePayrollValidator.cs
├── ValidatePayrollHandler.cs
└── ValidationResultDto.cs

src/FairWorkly.Infrastructure/Persistence/Repositories/Payroll/
├── PayslipRepository.cs
├── PayrollValidationRepository.cs
└── PayrollIssueRepository.cs

src/FairWorkly.API/Controllers/
└── PayrollController.cs

tests/FairWorkly.UnitTests/Integration/
└── PayrollValidationTests.cs
```

### 修改文件

- `src/FairWorkly.Application/Payroll/Interfaces/` - 添加 Repository 接口
- `src/FairWorkly.Infrastructure/DependencyInjection.cs` - 注册 Repository

---

## 核心流程

```
1. Controller 接收 multipart/form-data
2. 保存 CSV 到 wwwroot/uploads/{timestamp}_{filename}
3. 创建 PayrollValidation 记录 (状态: InProgress, 文件信息, 检查开关)
   → 详见 "PayrollValidation 生命周期" 章节
4. 解析 CSV → List<PayrollCsvRow>
5. 同步员工 → Dictionary<EmployeeNumber, EmployeeId>
6. 为每行创建 Payslip 记录 (带 PayrollValidationId 外键)
7. ⭐ Pre-Validation：检查必填字段完整性
   - 如果缺失 → 输出 WARNING，跳过该员工的规则检查
   - 如果完整 → 继续执行规则检查
8. 对每个 Payslip 执行 4 个规则检查 (根据开关控制)
9. 创建 PayrollIssue 记录
10. 更新 PayrollValidation (统计数据: TotalPayslips, PassedCount, FailedCount,
    TotalIssuesCount, CriticalIssuesCount, Status, CompletedAt)
    → 详见 "PayrollValidation 生命周期" 章节
11. 构建 ValidationResultDto 返回
```

> ⚠️ **架构说明**：根据 [ARCHITECTURE.md](../../.raw_materials/TECH_CONSTRAINTS/ARCHITECTURE.md)，Pre-Validation 是 **Handler 的职责**。
> Payroll 模块是纯规则计算，不需要 Orchestrator。Handler 直接调用 Service 和 ComplianceEngine Rules。

---

## 技术要点

### ValidatePayrollCommand

```csharp
public class ValidatePayrollCommand : IRequest<ValidationResultDto>
{
    public IFormFile File { get; set; }
    public string AwardType { get; set; }
    public string PayPeriod { get; set; }
    public DateOnly WeekStarting { get; set; }
    public DateOnly WeekEnding { get; set; }
    public string State { get; set; }
    public bool EnableBaseRateCheck { get; set; } = true;
    public bool EnablePenaltyCheck { get; set; } = true;
    public bool EnableCasualLoadingCheck { get; set; } = true;
    public bool EnableSuperCheck { get; set; } = true;
}
```

### ValidatePayrollValidator

```csharp
public class ValidatePayrollValidator : AbstractValidator<ValidatePayrollCommand>
{
    public ValidatePayrollValidator()
    {
        RuleFor(x => x.File).NotNull();
        RuleFor(x => x.AwardType).NotEmpty();
        RuleFor(x => x.PayPeriod).NotEmpty();
        RuleFor(x => x.WeekStarting).NotEmpty();
        RuleFor(x => x.WeekEnding).NotEmpty();
        RuleFor(x => x.State).NotEmpty();
    }
}
```

### Pre-Validation（在 Handler 中实现）

> 根据 [ARCHITECTURE.md](../../.raw_materials/TECH_CONSTRAINTS/ARCHITECTURE.md)，数据校验是 Handler 的职责。
>
> **v1.3 更新**：Pre-Validation 产出的 Issue 使用 `IssueCategory.PreValidation` 分类。

**检查字段**：
- Classification
- Employment Type
- Hourly Rate
- Ordinary Hours
- Ordinary Pay
- Gross Pay

**WarningMessage 模板**（根据 API Contract v1.3）：

| 场景 | WarningMessage 模板 |
|------|---------------------|
| 字段缺失/无效 | `Unable to verify: ${fieldName} is missing or invalid` |
| 格式错误 | `Unable to verify: ${fieldName} value '${value}' is not recognized` |

**处理逻辑**：
```csharp
// 在 ValidatePayrollHandler 中，对每个 Payslip 执行
private bool ValidatePayslipData(Payslip payslip, Guid validationId, List<PayrollIssue> issues)
{
    var missingFields = new List<string>();

    if (string.IsNullOrEmpty(payslip.Classification)) missingFields.Add("Classification");
    if (payslip.HourlyRate <= 0) missingFields.Add("Hourly Rate");
    if (payslip.OrdinaryHours < 0) missingFields.Add("Ordinary Hours");
    if (payslip.OrdinaryPay < 0) missingFields.Add("Ordinary Pay");
    if (payslip.GrossPay < 0) missingFields.Add("Gross Pay");

    if (missingFields.Any())
    {
        issues.Add(new PayrollIssue
        {
            OrganizationId = payslip.OrganizationId,
            PayrollValidationId = validationId,
            PayslipId = payslip.Id,
            EmployeeId = payslip.EmployeeId,
            CategoryType = IssueCategory.PreValidation,  // ← v1.3: 使用枚举
            Severity = IssueSeverity.Warning,
            WarningMessage = $"Unable to verify: {string.Join(", ", missingFields)} is missing or invalid",
            // 欠薪字段全部为 null
            ExpectedValue = null,
            ActualValue = null,
            AffectedUnits = null,
            UnitType = null,
            ContextLabel = null,
            ImpactAmount = 0  // Warning 无欠薪
        });
        return false; // Skip all rules for this employee
    }
    return true; // Continue with rule checks
}
```

### PayrollValidation 生命周期

PayrollValidation 采用**两阶段更新**模式，原因是 `Payslip.PayrollValidationId` 是外键，必须先创建 PayrollValidation 拿到 ID 后才能创建 Payslip。

#### 步骤 3 - 创建 PayrollValidation（初始数据）

```csharp
var validation = new PayrollValidation
{
    // 基础信息
    OrganizationId = MVP_ORGANIZATION_ID,  // 硬编码
    Status = ValidationStatus.InProgress,

    // 文件信息（步骤 2 后可用）
    FilePath = savedFilePath,
    FileName = command.File.FileName,

    // 时间范围（从 API 参数）
    PayPeriodStart = command.WeekStarting.ToDateTime(TimeOnly.MinValue),
    PayPeriodEnd = command.WeekEnding.ToDateTime(TimeOnly.MaxValue),

    // 执行时间
    StartedAt = _dateTimeProvider.Now,

    // 检查开关（从 API 参数）
    BaseRateCheckPerformed = command.EnableBaseRateCheck,
    PenaltyRateCheckPerformed = command.EnablePenaltyCheck,
    CasualLoadingCheckPerformed = command.EnableCasualLoadingCheck,
    SuperannuationCheckPerformed = command.EnableSuperCheck,

    // ⚠️ STP 是未来功能，当前硬编码 false
    STPCheckPerformed = false
};
```

> **注意**：`STPCheckPerformed` 是 Entity 中预留的未来功能占位符，API 尚未定义此开关，当前实现硬编码为 `false`。

#### 步骤 10 - 更新 PayrollValidation（统计数据）

所有检查完成后，更新统计字段：

```csharp
// 统计数据（步骤 8-9 完成后可计算）
validation.TotalPayslips = payslips.Count;
validation.PassedCount = payslips.Count(p => !issuesByPayslip.ContainsKey(p.Id));
validation.FailedCount = payslips.Count(p => issuesByPayslip.ContainsKey(p.Id));
validation.TotalIssuesCount = allIssues.Count;
validation.CriticalIssuesCount = allIssues.Count(i => i.Severity == IssueSeverity.Critical);

// 状态和完成时间
validation.Status = validation.TotalIssuesCount > 0
    ? ValidationStatus.Failed
    : ValidationStatus.Passed;
validation.CompletedAt = _dateTimeProvider.Now;
```

#### 字段计算时机总结

| 字段 | 计算时机 | 来源 |
|------|----------|------|
| `FilePath`, `FileName` | 步骤 2 后 | 文件保存结果 |
| `PayPeriodStart/End` | 步骤 3 | API 参数 |
| `*CheckPerformed` | 步骤 3 | API 参数开关 |
| `TotalPayslips` | 步骤 6 后 | Payslip 记录数 |
| `PassedCount`, `FailedCount` | 步骤 9 后 | Issue 统计 |
| `TotalIssuesCount`, `CriticalIssuesCount` | 步骤 9 后 | Issue 统计 |
| `CompletedAt` | 步骤 10 | 当前时间 |

### Entity ID 生成时机

> ⚠️ **重要**：`BaseEntity.Id` 在对象创建时是 `Guid.Empty`，需要先添加到 DbContext 才会生成 ID。

**EF Core 行为**：

| 时机 | Id 值 |
|------|-------|
| `new Payslip()` | `Guid.Empty` |
| `_context.Add(payslip)` | ✅ EF Core 自动生成 Guid |
| `SaveChangesAsync()` | 已有值，持久化到数据库 |

**流程要求**：

ComplianceEngine 规则创建 `PayrollIssue` 时需要 `payslip.Id`（参见 `BaseRateRule.CreateIssue()`）。

因此，**步骤 6 必须先将 Payslip 添加到 DbContext，再执行步骤 7-9 的规则检查**：

```csharp
// 步骤 6: 创建 Payslip 并添加到 Context（此时 Id 被生成）
var payslips = csvRows.Select(row => CreatePayslip(row, validation.Id, employeeMap)).ToList();
_context.Payslips.AddRange(payslips);  // ← Id 在这里自动生成

// 步骤 7-9: 现在可以安全地使用 payslip.Id
foreach (var payslip in payslips)
{
    if (!ValidatePayslipData(payslip, validation.Id, allIssues))
        continue;  // Pre-validation failed, skip rules

    foreach (var rule in _rules)
    {
        if (ShouldRunRule(rule, command))
        {
            var issues = rule.Evaluate(payslip, validation.Id);  // ← payslip.Id 有值
            allIssues.AddRange(issues);
        }
    }
}

// 步骤 9: 添加所有 Issue
_context.PayrollIssues.AddRange(allIssues);

// 步骤 10-11: 更新统计，一次性保存
await _context.SaveChangesAsync();
```

### ValidationResultDto（必须符合 API 契约 v1.3）

> **v1.3 更新**：
> - `Evidence` → `Description`（更直观）
> - 新增 `Warning` 字段（警告类使用）
> - `CategoryType` 新增 `"PreValidation"` 选项

```csharp
public class ValidationResultDto
{
    public Guid ValidationId { get; set; }
    public string Status { get; set; }  // "Passed" | "Failed"
    public DateTimeOffset Timestamp { get; set; }
    public SummaryDto Summary { get; set; }
    public List<CategoryDto> Categories { get; set; }
    public List<IssueDto> Issues { get; set; }
}

public class SummaryDto
{
    public int PassedCount { get; set; }
    public int TotalIssues { get; set; }
    public decimal TotalUnderpayment { get; set; }
    public int AffectedEmployees { get; set; }
}

public class CategoryDto
{
    public string Key { get; set; }  // "PreValidation" | "BaseRate" | "PenaltyRate" | "Superannuation" | "CasualLoading"
    public int AffectedEmployeeCount { get; set; }
    public decimal TotalUnderpayment { get; set; }  // PreValidation 和警告类为 0
}

public class IssueDto
{
    public Guid IssueId { get; set; }
    public string CategoryType { get; set; }  // "PreValidation" | "BaseRate" | "PenaltyRate" | "Superannuation" | "CasualLoading"
    public string EmployeeName { get; set; }
    public string EmployeeId { get; set; }
    public string IssueStatus { get; set; }  // "OPEN" | "RESOLVED"
    public int Severity { get; set; }  // 1: Info, 2: Warning, 3: Error, 4: Critical
    public decimal ImpactAmount { get; set; }  // 警告类为 0
    public DescriptionDto? Description { get; set; }  // ← v1.3: 欠薪类使用（原 Evidence）
    public string? Warning { get; set; }  // ← v1.3 新增: 警告类使用
}

// v1.3: Evidence 重命名为 Description
public class DescriptionDto
{
    public decimal ActualValue { get; set; }
    public decimal ExpectedValue { get; set; }
    public decimal AffectedUnits { get; set; }
    public string UnitType { get; set; }  // "Hour" | "Currency"
    public string ContextLabel { get; set; }
}
```

### Description vs Warning 互斥规则

| Issue 类型 | Severity | Description | Warning |
|------------|----------|-------------|---------|
| 欠薪类 | 3 (Error) / 4 (Critical) | ✅ 填充 | `null` |
| 警告类 | 2 (Warning) | `null` | ✅ 填充 |

**映射逻辑**：
```csharp
// 构建 IssueDto 时
private IssueDto MapToIssueDto(PayrollIssue issue, Employee employee)
{
    var isWarning = issue.Severity == IssueSeverity.Warning;

    return new IssueDto
    {
        IssueId = issue.Id,
        CategoryType = issue.CategoryType.ToString(),  // 枚举转字符串
        EmployeeName = employee.Name,
        EmployeeId = employee.EmployeeNumber,
        IssueStatus = issue.IsResolved ? "RESOLVED" : "OPEN",
        Severity = (int)issue.Severity,
        ImpactAmount = issue.ImpactAmount ?? 0,

        // 互斥逻辑
        Description = isWarning ? null : new DescriptionDto
        {
            ActualValue = issue.ActualValue ?? 0,
            ExpectedValue = issue.ExpectedValue ?? 0,
            AffectedUnits = issue.AffectedUnits ?? 0,
            UnitType = issue.UnitType ?? "Hour",
            ContextLabel = issue.ContextLabel ?? ""
        },
        Warning = isWarning ? issue.WarningMessage : null
    };
}
```

### PayrollController

```csharp
[ApiController]
[Route("api/payroll")]
public class PayrollController : ControllerBase
{
    private readonly IMediator _mediator;

    [HttpPost("validation")]
    public async Task<IActionResult> Validate([FromForm] ValidatePayrollCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new
        {
            code = 200,
            msg = "Audit completed successfully",
            data = result
        });
    }
}
```

---

## API 响应结构（v1.3）

> **v1.3 更新**：
> - `evidence` → `description`
> - 新增 `warning` 字段
> - `categories.key` 新增 `"PreValidation"`

```json
{
  "code": 200,
  "msg": "Audit completed successfully",
  "data": {
    "validationId": "VAL-17029384",
    "status": "Failed",
    "timestamp": "2025-12-18T20:00:00Z",
    "summary": {
      "passedCount": 85,
      "totalIssues": 4,
      "totalUnderpayment": 327.58,
      "affectedEmployees": 4
    },
    "categories": [
      {
        "key": "PreValidation",
        "affectedEmployeeCount": 1,
        "totalUnderpayment": 0
      },
      {
        "key": "BaseRate",
        "affectedEmployeeCount": 2,
        "totalUnderpayment": 180.80
      },
      {
        "key": "Superannuation",
        "affectedEmployeeCount": 1,
        "totalUnderpayment": 50.00
      }
    ],
    "issues": [
      {
        "issueId": "f1e2d3c4-...",
        "categoryType": "PreValidation",
        "employeeName": "Tom Wilson",
        "employeeId": "E003",
        "issueStatus": "OPEN",
        "severity": 2,
        "impactAmount": 0,
        "description": null,
        "warning": "Unable to verify: Classification is missing or invalid"
      },
      {
        "issueId": "a1b2c3d4-...",
        "categoryType": "BaseRate",
        "employeeName": "Jack Smith",
        "employeeId": "E001",
        "issueStatus": "OPEN",
        "severity": 4,
        "impactAmount": 76.40,
        "description": {
          "actualValue": 23.50,
          "expectedValue": 25.41,
          "affectedUnits": 40.0,
          "unitType": "Hour",
          "contextLabel": "Retail Award Level 2"
        },
        "warning": null
      },
      {
        "issueId": "d4e5f6a7-...",
        "categoryType": "Superannuation",
        "employeeName": "Sarah Davis",
        "employeeId": "E047",
        "issueStatus": "OPEN",
        "severity": 3,
        "impactAmount": 50.00,
        "description": {
          "actualValue": 250.00,
          "expectedValue": 300.00,
          "affectedUnits": 2500.00,
          "unitType": "Currency",
          "contextLabel": "12%"
        },
        "warning": null
      }
    ]
  }
}
```

---

## 关键约束

1. **使用 CQRS 模式**：Command → Validator → Handler
2. **使用 FluentValidation** 进行输入校验
3. **整个流程在同一个事务中完成**
4. **使用 `IDateTimeProvider`** 而非 `DateTime.Now`
5. **MVP 阶段 OrganizationId 硬编码**
6. **CSV 文件持久化保存到 `wwwroot/uploads/`**

---

## CategoryType 映射（v1.3 更新）

> **v1.3 更新**：新增 `PreValidation` 分类。

| 来源 | IssueCategory 枚举 | API CategoryType |
|------|-------------------|------------------|
| Handler Pre-Validation | `IssueCategory.PreValidation` | `"PreValidation"` |
| BaseRateRule | `IssueCategory.BaseRate` | `"BaseRate"` |
| PenaltyRateRule | `IssueCategory.PenaltyRate` | `"PenaltyRate"` |
| CasualLoadingRule | `IssueCategory.CasualLoading` | `"CasualLoading"` |
| SuperannuationRule | `IssueCategory.Superannuation` | `"Superannuation"` |

### categoryType 分类规则

| 场景 | categoryType | 说明 |
|------|--------------|------|
| Rule 1 检查发现的问题（含警告） | **BaseRate** | 包括欠薪和数据异常 |
| Rule 2 检查发现的问题 | **PenaltyRate** | Saturday/Sunday/Public Holiday |
| Rule 3 检查发现的问题（含警告） | **CasualLoading** | 包括欠薪和配置风险 |
| Rule 4 检查发现的问题（含警告） | **Superannuation** | 包括欠薪和缺数据 |
| PreValidation 无法解析 | **PreValidation** | 字段缺失/格式错误，无法进入任何 Rule |

> **注意**：警告类（severity=2）归属到**发现问题的那个 Rule 对应的分类**，只有 Handler Pre-Validation 阶段发现的问题才归到 PreValidation 分类。

---

## 验收标准

- [ ] API 端点 `POST /api/payroll/validation` 可访问
- [ ] 上传 CSV 后返回完整的审计结果
- [ ] Response 结构符合 API 契约
- [ ] 4 个开关（enableXxxCheck）能正确控制规则执行
- [ ] PayrollValidation 记录正确创建
- [ ] Payslip 记录包含正确的快照数据
- [ ] PayrollIssue 记录包含完整的 Evidence 数据
- [ ] Summary 统计数据准确
- [ ] CSV 格式错误返回 400 + 详细错误信息
- [ ] 集成测试通过 (TC-INT-001~004)

---

## 对应测试

| 测试用例 | CSV 文件 | 验证目标 |
|----------|----------|----------|
| TC-INT-001 | TEST_13_AllCompliant.csv | 全部合规，status="Passed" |
| TC-INT-002 | TEST_14_AllViolations.csv | 全部违规，4 种 Category 都有 |
| TC-INT-003 | TEST_15_MixedScenarios.csv | 真实混合场景 |
| TC-INT-004 | TEST_16_EdgeCases.csv | 边界值测试 |

---

## 依赖

- **前置 Issue**: ISSUE_01, ISSUE_02
- **确认 DI 注册**: CSV 服务、同步服务、4 个规则
