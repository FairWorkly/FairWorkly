# Payroll Module AI_GUIDE

> **Payroll 模块导航。薪资合规审计的核心业务逻辑。**

---

## 概述

Payroll 模块负责：
1. 解析用户上传的薪资 CSV
2. 同步员工数据
3. 执行 4 个合规规则检查
4. 输出违规报告

---

## 目录结构（计划）

```
Payroll/
├── Features/
│   └── ValidatePayroll/           ← CQRS Handler（ISSUE_03）
│       ├── ValidatePayrollCommand.cs
│       ├── ValidatePayrollValidator.cs
│       ├── ValidatePayrollHandler.cs
│       └── ValidationResultDto.cs
├── Services/
│   ├── CsvParserService.cs        ← CSV 解析（ISSUE_01）
│   ├── EmployeeSyncService.cs     ← 员工同步（ISSUE_01）
│   ├── Models/
│   │   └── PayrollCsvRow.cs
│   └── ComplianceEngine/          ← 合规规则（ISSUE_02）
│       ├── IComplianceRule.cs
│       ├── BaseRateRule.cs
│       ├── PenaltyRateRule.cs
│       ├── CasualLoadingRule.cs
│       ├── SuperannuationRule.cs
│       └── RateTableProvider.cs
├── Interfaces/
│   ├── ICsvParserService.cs
│   ├── IEmployeeSyncService.cs
│   ├── IPayslipRepository.cs
│   ├── IPayrollValidationRepository.cs
│   └── IPayrollIssueRepository.cs
└── Orchestrators/
    └── PayrollAiOrchestrator.cs   ← AI 编排（已存在）
```

---

## 核心业务流程

```
CSV上传
    ↓
CsvParserService.ParseAsync()
    ↓
EmployeeSyncService.SyncEmployeesAsync()
    ↓
创建 Payslip 记录
    ↓
┌─────────────────────────────────────┐
│         ComplianceEngine            │
│  BaseRateRule → PenaltyRateRule    │
│  CasualLoadingRule → SuperRule     │
└─────────────────────────────────────┘
    ↓
创建 PayrollIssue 记录
    ↓
返回 ValidationResultDto
```

---

## 开发任务

| Issue | 名称 | 状态 | 详情 |
|-------|------|------|------|
| ISSUE_01 | CSV 解析 + 员工同步 | 待开发 | [/.doc/issues/ISSUE_01_CsvParser_EmployeeSync.md](/.doc/issues/ISSUE_01_CsvParser_EmployeeSync.md) |
| ISSUE_02 | 合规规则引擎 | 待开发 | [/.doc/issues/ISSUE_02_ComplianceEngine.md](/.doc/issues/ISSUE_02_ComplianceEngine.md) |
| ISSUE_03 | Handler 集成 + API | 待开发 | [/.doc/issues/ISSUE_03_Handler_API.md](/.doc/issues/ISSUE_03_Handler_API.md) |

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

- [/.doc/SPEC_Payroll.md](/.doc/SPEC_Payroll.md) - 模块技术规格
- [/.doc/TEST_PLAN.md](/.doc/TEST_PLAN.md) - 测试方案
- [/.raw_materials/BUSINESS_RULES/Payroll_Engine_Logic.md.md](/.raw_materials/BUSINESS_RULES/Payroll_Engine_Logic.md.md) - 业务规则（只读）
