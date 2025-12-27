# ISSUE_03: Handler 集成 + API 暴露

## 目标

集成所有组件，实现完整的验证流程并暴露 API 接口。

**一句话**：把 CSV 解析、员工同步、合规检查串联起来，通过 API 返回完整的审计报告。

---

## 状态

- [ ] 开发中
- [ ] 测试通过
- [ ] Review 完成

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
3. 创建 PayrollValidation 记录 (状态: InProgress)
4. 解析 CSV → List<PayrollCsvRow>
5. 同步员工 → Dictionary<EmployeeNumber, EmployeeId>
6. 为每行创建 Payslip 记录
7. 对每个 Payslip 执行 4 个规则检查 (根据开关控制)
8. 创建 PayrollIssue 记录
9. 更新 PayrollValidation (状态: Passed/Failed, 统计数据)
10. 构建 ValidationResultDto 返回
```

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

### ValidationResultDto（必须符合 API 契约）

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
    public string Key { get; set; }  // "BaseRate" | "PenaltyRate" | "Superannuation" | "CasualLoading"
    public int AffectedEmployeeCount { get; set; }
    public decimal TotalUnderpayment { get; set; }
}

public class IssueDto
{
    public Guid IssueId { get; set; }
    public string CategoryType { get; set; }
    public string EmployeeName { get; set; }
    public string EmployeeId { get; set; }
    public string IssueStatus { get; set; }  // "OPEN" | "RESOLVED"
    public int Severity { get; set; }  // 1-4
    public decimal ImpactAmount { get; set; }
    public EvidenceDto Evidence { get; set; }
}

public class EvidenceDto
{
    public decimal ActualValue { get; set; }
    public decimal ExpectedValue { get; set; }
    public decimal AffectedUnits { get; set; }
    public string UnitType { get; set; }  // "Hour" | "Currency"
    public string ContextLabel { get; set; }
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

## API 响应结构

```json
{
  "code": 200,
  "msg": "Audit completed successfully",
  "data": {
    "validationId": "GUID",
    "status": "Passed | Failed",
    "timestamp": "ISO8601",
    "summary": {
      "passedCount": 85,
      "totalIssues": 15,
      "totalUnderpayment": 1847.00,
      "affectedEmployees": 5
    },
    "categories": [
      {
        "key": "BaseRate",
        "affectedEmployeeCount": 3,
        "totalUnderpayment": 500.50
      }
    ],
    "issues": [
      {
        "issueId": "GUID",
        "categoryType": "BaseRate",
        "employeeName": "Jack Smith",
        "employeeId": "E001",
        "issueStatus": "OPEN",
        "severity": 4,
        "impactAmount": 76.40,
        "evidence": {
          "actualValue": 23.50,
          "expectedValue": 25.41,
          "affectedUnits": 40.0,
          "unitType": "Hour",
          "contextLabel": "Retail Award Level 2"
        }
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

## CategoryType 映射

| 规则 | CategoryType |
|------|--------------|
| BaseRateRule | "BaseRate" |
| PenaltyRateRule | "PenaltyRate" |
| CasualLoadingRule | "CasualLoading" |
| SuperannuationRule | "Superannuation" |

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
