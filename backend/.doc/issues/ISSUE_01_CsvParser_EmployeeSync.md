# ISSUE_01: CSV 解析 + 员工同步

## 目标

实现 CSV 文件解析和员工数据同步（Upsert）功能。

**一句话**：把用户上传的薪资 CSV 解析成结构化数据，并将员工信息同步到 Employee 表。

---

## 状态

- [ ] 开发中
- [ ] 测试通过
- [ ] Review 完成

---

## 输入与输出

- **输入**：用户上传的 CSV 文件流
- **输出**：
  - 解析后的薪资记录列表 `List<PayrollCsvRow>`
  - 员工 EmployeeNumber → EmployeeId 的映射字典

---

## 交付物

### 新建文件

```
src/FairWorkly.Application/Payroll/
├── Interfaces/
│   ├── ICsvParserService.cs
│   └── IEmployeeSyncService.cs
├── Services/
│   ├── CsvParserService.cs
│   ├── EmployeeSyncService.cs
│   └── Models/
│       └── PayrollCsvRow.cs

src/FairWorkly.Infrastructure/Persistence/Repositories/
└── Payroll/
    └── EmployeeRepository.cs

tests/FairWorkly.UnitTests/Unit/
├── CsvParserServiceTests.cs
└── EmployeeSyncServiceTests.cs
```

### 修改文件

- `src/FairWorkly.Application/DependencyInjection.cs` - 注册新服务
- `src/FairWorkly.Infrastructure/DependencyInjection.cs` - 注册 Repository

---

## CSV 字段映射

| CSV Header | 必填 | 类型 | 映射目标 |
|------------|------|------|----------|
| Employee ID | ✅ | string | EmployeeNumber |
| Employee Name | ✅ | string | 拆分为 FirstName + LastName |
| Pay Period Start | ✅ | Date | PayPeriodStart |
| Pay Period End | ✅ | Date | PayPeriodEnd |
| Award Type | ✅ | string | AwardType (需解析) |
| Classification | ✅ | string | AwardLevelNumber (解析 Level X) |
| Employment Type | ✅ | string | EmploymentType (Enum) |
| Hourly Rate | ✅ | decimal | HourlyRate |
| Ordinary Hours | ✅ | decimal | OrdinaryHours |
| Ordinary Pay | ✅ | decimal | OrdinaryPay |
| Saturday Hours | ❌ | decimal | SaturdayHours (默认 0) |
| Saturday Pay | ❌ | decimal | SaturdayPay |
| Sunday Hours | ❌ | decimal | SundayHours (默认 0) |
| Sunday Pay | ❌ | decimal | SundayPay |
| Public Holiday Hours | ❌ | decimal | PublicHolidayHours (默认 0) |
| Public Holiday Pay | ❌ | decimal | PublicHolidayPay |
| Gross Pay | ✅ | decimal | GrossPay |
| Superannuation Paid | ✅ | decimal | Superannuation |

---

## 关键约束

1. **使用 CsvHelper NuGet 包**
2. **接口定义在 Application 层，实现在 Application 层**
3. **员工同步逻辑**：根据 `EmployeeNumber + OrganizationId` 判断是新建还是更新
4. **OrganizationId**：MVP 阶段硬编码固定 GUID

---

## 技术要点

### CsvParserService

```csharp
public interface ICsvParserService
{
    Task<(List<PayrollCsvRow> Rows, List<string> Errors)> ParseAsync(
        Stream csvStream,
        CancellationToken cancellationToken = default);
}
```

- 使用 CsvHelper 解析
- 必填字段缺失时，记录错误但继续处理其他行
- 可选字段缺失时，使用默认值 0
- 返回解析成功的行 + 错误列表

### EmployeeSyncService

```csharp
public interface IEmployeeSyncService
{
    Task<Dictionary<string, Guid>> SyncEmployeesAsync(
        List<PayrollCsvRow> rows,
        Guid organizationId,
        CancellationToken cancellationToken = default);
}
```

- 根据 EmployeeNumber 查询现有员工
- 存在则更新，不存在则创建
- 返回 EmployeeNumber → EmployeeId 映射

### PayrollCsvRow 模型

```csharp
public class PayrollCsvRow
{
    public string EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public DateOnly PayPeriodStart { get; set; }
    public DateOnly PayPeriodEnd { get; set; }
    public string AwardType { get; set; }
    public string Classification { get; set; }
    public string EmploymentType { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal OrdinaryHours { get; set; }
    public decimal OrdinaryPay { get; set; }
    public decimal SaturdayHours { get; set; }
    public decimal SaturdayPay { get; set; }
    public decimal SundayHours { get; set; }
    public decimal SundayPay { get; set; }
    public decimal PublicHolidayHours { get; set; }
    public decimal PublicHolidayPay { get; set; }
    public decimal GrossPay { get; set; }
    public decimal SuperannuationPaid { get; set; }
}
```

---

## 验收标准

- [ ] CSV 能正确解析所有字段
- [ ] 必填字段缺失时，记录错误但继续处理其他行
- [ ] 可选字段缺失时，使用默认值 0
- [ ] 新员工能正确创建
- [ ] 已有员工能正确更新（Upsert）
- [ ] 返回完整的 EmployeeNumber → EmployeeId 映射
- [ ] 服务已在 DependencyInjection.cs 中注册
- [ ] 单元测试通过 (TC-SYNC-001~003)

---

## 对应测试

| 测试用例 | CSV 文件 | 验证目标 |
|----------|----------|----------|
| TC-SYNC-001 | TEST_01_NewEmployees.csv | 创建新员工 |
| TC-SYNC-002 | TEST_02_UpdateEmployees.csv | 更新已有员工 |
| TC-SYNC-003 | TEST_03_MixedEmployees.csv | 混合场景 |

---

## 依赖

- **NuGet**: `CsvHelper`
- **前置 Issue**: 无（这是第一个任务）
