# Tests AI_GUIDE

> **测试目录导航。包含单元测试和集成测试。**

> ⚠️ **宪法文档提醒**：测试用例应覆盖 [.raw_materials/BUSINESS_RULES/](../../.raw_materials/BUSINESS_RULES/) 中定义的所有业务规则。
> 费率表、计算逻辑的预期值必须与宪法文档一致。

---

## 概述

测试项目包含：
- **单元测试**: 隔离测试各个服务组件
- **集成测试**: 测试组件协作和数据库交互

---

## 目录结构

```
FairWorkly.UnitTests/
├── Unit/                            ← 单元测试
│   ├── CsvParserServiceTests.cs     ✅ 7 tests (ISSUE_01)
│   ├── EmployeeSyncServiceTests.cs  ✅ 6 tests (ISSUE_01)
│   ├── BaseRateRuleTests.cs         ✅ 13 tests (ISSUE_02)
│   ├── PenaltyRateRuleTests.cs      ✅ 13 tests (ISSUE_02)
│   ├── CasualLoadingRuleTests.cs    ✅ 17 tests (ISSUE_02)
│   └── SuperannuationRuleTests.cs   ✅ 22 tests (ISSUE_02)
├── Integration/                     ← 集成测试
│   ├── EmployeeSyncIntegrationTests.cs ✅ 3 tests (ISSUE_01)
│   └── PayrollValidationTests.cs    ⏳ 待 ISSUE_03
├── TestData/
│   └── Csv/                         ← 测试 CSV 文件
│       ├── TEST_01_NewEmployees.csv
│       ├── TEST_02_UpdateEmployees.csv
│       └── ... (16 个测试文件)
└── AI_GUIDE.md                      ← 本文件
```

---

## 测试进度

| Issue | 测试类 | 当前 | 目标 | 状态 |
|-------|--------|------|------|------|
| ISSUE_01 | CsvParserServiceTests | 7 | 12 | +5 待补充 |
| ISSUE_01 | EmployeeSyncServiceTests | 6 | 6 | ✅ Pass |
| ISSUE_01 | EmployeeSyncIntegrationTests | 3 | 3 | ✅ Pass |
| ISSUE_02 | BaseRateRuleTests | 13 | 17 | +4 待补充 |
| ISSUE_02 | PenaltyRateRuleTests | 13 | 13 | ✅ Pass |
| ISSUE_02 | CasualLoadingRuleTests | 17 | 17 | ✅ Pass |
| ISSUE_02 | SuperannuationRuleTests | 22 | 22 | ✅ Pass |
| ISSUE_03 | ValidatePayrollHandlerTests | - | 3 | ⏳ 待实现 |
| ISSUE_03 | PayrollValidationTests | - | 4 | ⏳ 待实现 |
| **总计** | - | **81** | **97** | **81/97 Pass** |

### 待补充测试（Phase 2）

| 测试类 | 新增数 | 场景 |
|--------|--------|------|
| CsvParserServiceTests | +5 | 错误场景（空文件、格式错误等） |
| BaseRateRuleTests | +4 | Level 4-8 边界测试 |

> 详细测试用例见 [TEST_PLAN.md](../../.doc/TEST_PLAN.md#8-测试统计)

---

## 测试框架

| 组件 | 技术 |
|------|------|
| 测试框架 | xUnit |
| 断言库 | FluentAssertions |
| Mock | Moq |
| 内存数据库 | Microsoft.EntityFrameworkCore.InMemory |

---

## 测试环境

| 测试类型 | 数据库 | 说明 |
|----------|--------|------|
| 单元测试 | InMemory | 快速、隔离 |
| 集成测试 | PostgreSQL | 真实数据库行为 |

---

## 测试 CSV 文件说明

### ISSUE_01 测试 (员工同步)

| 文件 | 员工数 | 测试目标 |
|------|--------|----------|
| TEST_01_NewEmployees.csv | 5 | 创建新员工 |
| TEST_02_UpdateEmployees.csv | 3 | 更新已有员工 |
| TEST_03_MixedEmployees.csv | 6 | 混合场景 |

### ISSUE_02 测试 (合规规则)

| 文件 | 规则 | 预期结果 |
|------|------|----------|
| TEST_04_BaseRate_AllPass.csv | BaseRate | 0 问题 |
| TEST_05_BaseRate_Violations.csv | BaseRate | 4 CRITICAL |
| TEST_06_Saturday_Violations.csv | PenaltyRate | 3 ERROR |
| TEST_07_Sunday_Violations.csv | PenaltyRate | 3 ERROR |
| TEST_08_PublicHoliday_Violations.csv | PenaltyRate | 3 ERROR |
| TEST_09_Casual_AllPass.csv | CasualLoading | 0 问题 |
| TEST_10_Casual_Violations.csv | CasualLoading | 混合 |
| TEST_11_Super_AllPass.csv | Super | 0 问题 |
| TEST_12_Super_Violations.csv | Super | 4 ERROR |

### ISSUE_03 测试 (集成)

| 文件 | 场景 |
|------|------|
| TEST_13_AllCompliant.csv | 全部合规 |
| TEST_14_AllViolations.csv | 全部违规 |
| TEST_15_MixedScenarios.csv | 真实混合 |
| TEST_16_EdgeCases.csv | 边界值 |

---

## 运行测试

```bash
# 运行所有测试
dotnet test

# 运行特定测试类
dotnet test --filter "FullyQualifiedName~CsvParserServiceTests"

# 运行特定测试方法
dotnet test --filter "FullyQualifiedName~CsvParserServiceTests.ParseAsync_ValidCsv_ReturnsRows"

# 集成测试前清库
dotnet ef database drop --force --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API
dotnet ef database update --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API
```

---

## 相关文档

- [/.doc/TEST_PLAN.md](../../.doc/TEST_PLAN.md) - 测试方案详情
- [/.doc/SPEC_Payroll.md](../../.doc/SPEC_Payroll.md) - 业务规则详情
- [/.raw_materials/BUSINESS_RULES/Payroll_Engine_Logic.md](../../.raw_materials/BUSINESS_RULES/Payroll_Engine_Logic.md) - 费率表和计算逻辑（只读）

---

## 文档矩阵链接

### 上级导航
- [← 返回仓库级 AI_GUIDE](../../AI_GUIDE.md)
- [← .doc/AI_GUIDE.md](../../.doc/AI_GUIDE.md) - 项目状态

### 被测试的代码
- [Application 层](../../src/FairWorkly.Application/AI_GUIDE.md)
- [Payroll 模块](../../src/FairWorkly.Application/Payroll/AI_GUIDE.md) ← **当前开发重点**
- [Infrastructure 层](../../src/FairWorkly.Infrastructure/AI_GUIDE.md)

### Issue 文档
- [ISSUE_01 (已完成)](../../.doc/issues/ISSUE_01_CsvParser_EmployeeSync.md) - 16 tests
- [ISSUE_02 (已完成)](../../.doc/issues/ISSUE_02_ComplianceEngine.md) - 65 tests
- [ISSUE_03 (当前)](../../.doc/issues/ISSUE_03_Handler_API.md) ← **下一个任务**

---

*最后更新: 2026-01-04 (测试评估报告 Phase 1, 81 tests → 目标 97 tests)*
