# Payroll Module AI_GUIDE

> **Payroll 模块导航。薪资合规审计的核心业务逻辑。**

> ⚠️ **宪法文档提醒**：业务规则定义在 [.raw_materials/BUSINESS_RULES/](../../../.raw_materials/BUSINESS_RULES/)，这是绝对只读的。
> 费率表、API 契约不能修改。如有异议，向人类报告。

---

## 概述

Payroll 模块负责：
1. 解析用户上传的薪资 CSV
2. 同步员工数据
3. 执行 4 个合规规则检查
4. 输出违规报告

---

## 目录结构（当前状态）

```
Payroll/
├── Interfaces/
│   ├── ICsvParserService.cs          ✅ 已实现
│   ├── IEmployeeSyncService.cs       ✅ 已实现
│   ├── IPayslipRepository.cs         ⚠️ 空壳，待 ISSUE_03
│   ├── IPayrollValidationRepository.cs ⚠️ 空壳，待 ISSUE_03
│   └── IPayrollIssueRepository.cs    ⚠️ 空壳，待 ISSUE_03
├── Services/
│   ├── CsvParserService.cs           ✅ 已实现 (ISSUE_01)
│   ├── EmployeeSyncService.cs        ✅ 已实现 (ISSUE_01)
│   ├── Models/
│   │   └── PayrollCsvRow.cs          ✅ 已实现
│   └── ComplianceEngine/             ⏳ 待实现 (ISSUE_02)
│       ├── IComplianceRule.cs
│       ├── BaseRateRule.cs
│       ├── PenaltyRateRule.cs
│       ├── CasualLoadingRule.cs
│       ├── SuperannuationRule.cs
│       └── RateTableProvider.cs
├── Features/
│   └── ValidatePayroll/              ⏳ 待实现 (ISSUE_03)
│       ├── ValidatePayrollCommand.cs
│       ├── ValidatePayrollValidator.cs
│       ├── ValidatePayrollHandler.cs
│       └── ValidationResultDto.cs
├── Orchestrators/
│   └── PayrollAiOrchestrator.cs      ⚠️ 骨架
└── AI_GUIDE.md                       ← 本文件
```

---

## 开发进度

| Issue | 名称 | 状态 | 详情 |
|-------|------|------|------|
| ISSUE_01 | CSV 解析 + 员工同步 | ✅ 完成 | [详情](/../.doc/issues/ISSUE_01_CsvParser_EmployeeSync.md) |
| ISSUE_02 | 合规规则引擎 | ⏳ **当前任务** | [详情](/../.doc/issues/ISSUE_02_ComplianceEngine.md) |
| ISSUE_03 | Handler 集成 + API | ⏳ 待开发 | [详情](/../.doc/issues/ISSUE_03_Handler_API.md) |

---

## 核心业务流程

```
CSV上传
    ↓
CsvParserService.ParseAsync()           ✅ 已实现
    ↓
EmployeeSyncService.SyncEmployeesAsync() ✅ 已实现
    ↓
创建 Payslip 记录                        ⏳ ISSUE_03
    ↓
┌─────────────────────────────────────┐
│         ComplianceEngine            │  ⏳ ISSUE_02
│  BaseRateRule → PenaltyRateRule    │
│  CasualLoadingRule → SuperRule     │
└─────────────────────────────────────┘
    ↓
创建 PayrollIssue 记录                   ⏳ ISSUE_03
    ↓
返回 ValidationResultDto                 ⏳ ISSUE_03
```

---

## 已实现的代码详情

### CsvParserService

**文件**: `Services/CsvParserService.cs`

**功能**:
- 使用 CsvHelper 解析 CSV 文件
- 映射 18 个字段到 `PayrollCsvRow`
- 必填字段缺失时记录错误，继续处理
- 可选字段缺失使用默认值 0

**接口**:
```csharp
Task<(List<PayrollCsvRow> Rows, List<string> Errors)> ParseAsync(
    Stream csvStream,
    CancellationToken cancellationToken = default);
```

### EmployeeSyncService

**文件**: `Services/EmployeeSyncService.cs`

**功能**:
- Upsert 员工数据 (根据 EmployeeNumber + OrganizationId)
- 解析员工姓名 (空格分隔)
- 解析 AwardType、Classification、EmploymentType
- 新员工使用占位符 Email

**接口**:
```csharp
Task<Dictionary<string, Guid>> SyncEmployeesAsync(
    List<PayrollCsvRow> rows,
    Guid organizationId,
    CancellationToken cancellationToken = default);
```

---

## ISSUE_02 待实现详情

### RateTableProvider (静态费率表)

```csharp
public static class RateTableProvider
{
    // Permanent Rate (Level 1-8)
    public static readonly Dictionary<int, decimal> PermanentRates = new()
    {
        { 1, 26.55m }, { 2, 27.16m }, { 3, 27.58m }, { 4, 28.12m },
        { 5, 29.27m }, { 6, 29.70m }, { 7, 31.19m }, { 8, 32.45m }
    };

    // Casual Rate (含 25% Loading)
    public static readonly Dictionary<int, decimal> CasualRates = new()
    {
        { 1, 33.19m }, { 2, 33.95m }, { 3, 34.48m }, { 4, 35.15m },
        { 5, 36.59m }, { 6, 37.13m }, { 7, 38.99m }, { 8, 40.56m }
    };

    // 罚金倍率
    public static class Multipliers { ... }

    // 养老金率
    public const decimal SuperannuationRate = 0.12m;
}
```

### 4 个合规规则

| 规则 | 检查目标 | Severity |
|------|----------|----------|
| BaseRateRule | 时薪 >= Permanent Rate | CRITICAL / WARNING |
| PenaltyRateRule | 周末/公休罚金正确 | ERROR |
| CasualLoadingRule | Casual 获得 25% Loading | CRITICAL / WARNING |
| SuperannuationRule | 养老金 >= 12% | ERROR / WARNING |

---

## 依赖的 Entity（不可修改）

| Entity | 位置 | 用途 |
|--------|------|------|
| Payslip | Domain/Payroll/Entities | 薪资快照记录 |
| PayrollValidation | Domain/Payroll/Entities | 验证批次记录 |
| PayrollIssue | Domain/Payroll/Entities | 违规问题记录 |
| Employee | Domain/Employees/Entities | 员工信息 |

---

## 相关文档

- [/.doc/SPEC_Payroll.md](/../.doc/SPEC_Payroll.md) - 模块技术规格
- [/.doc/TEST_PLAN.md](/../.doc/TEST_PLAN.md) - 测试方案
- [/.raw_materials/BUSINESS_RULES/Payroll_Engine_Logic.md](/../.raw_materials/BUSINESS_RULES/Payroll_Engine_Logic.md) - 业务规则（只读）
- [/.raw_materials/BUSINESS_RULES/API_Contract.md](/../.raw_materials/BUSINESS_RULES/API_Contract.md) - API 契约（只读）

---

## 文档矩阵链接

### 上级导航
- [← 返回 Application 层](../AI_GUIDE.md)
- [← 返回仓库级 AI_GUIDE](../../../AI_GUIDE.md)
- [← .doc/AI_GUIDE.md](../../../.doc/AI_GUIDE.md) - 项目状态

### Issue 文档
- [ISSUE_01 (已完成)](../../../.doc/issues/ISSUE_01_CsvParser_EmployeeSync.md)
- [ISSUE_02 (当前)](../../../.doc/issues/ISSUE_02_ComplianceEngine.md) ← **当前任务**
- [ISSUE_03 (待开发)](../../../.doc/issues/ISSUE_03_Handler_API.md)

### 规格文档
- [SPEC_Payroll.md](../../../.doc/SPEC_Payroll.md) - 技术规格
- [TEST_PLAN.md](../../../.doc/TEST_PLAN.md) - 测试方案

### 宪法文档 (只读)
- [Payroll_Engine_Logic.md](../../../.raw_materials/BUSINESS_RULES/Payroll_Engine_Logic.md) - 费率表
- [API_Contract.md](../../../.raw_materials/BUSINESS_RULES/API_Contract.md) - API 契约

---

*最后更新: 2026-01-01*
