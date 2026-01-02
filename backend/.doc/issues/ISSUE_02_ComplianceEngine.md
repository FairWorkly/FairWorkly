# ISSUE_02: 合规规则引擎

> ✅ **已完成**：此 Issue 已于 2026-01-01 完成

## 目标

实现 4 个薪资合规检查规则引擎。

**一句话**：检查每条薪资记录是否符合 Retail Award 的费率规定，识别违规并计算差额。

---

## 状态

- [x] 开发中
- [x] 测试通过 (81 tests, 65 new for ComplianceEngine)
- [x] Review 完成 ← **已完成**

**前置依赖**: ISSUE_01 ✅ 已完成

---

## 输入与输出

- **输入**：Payslip 实体（包含员工信息、工时、工资等）
- **输出**：PayrollIssue 列表（违规问题，包含 Severity、差额、Evidence）

---

## 交付物

### 新建文件

```
src/FairWorkly.Application/Payroll/Services/ComplianceEngine/
├── IComplianceRule.cs
├── BaseRateRule.cs
├── PenaltyRateRule.cs
├── CasualLoadingRule.cs
├── SuperannuationRule.cs
└── RateTableProvider.cs

tests/FairWorkly.UnitTests/Unit/
├── BaseRateRuleTests.cs
├── PenaltyRateRuleTests.cs
├── CasualLoadingRuleTests.cs
└── SuperannuationRuleTests.cs
```

### 修改文件

- `src/FairWorkly.Application/DependencyInjection.cs` - 注册规则服务

---

## AI Commit 权限

> **本章节授权 AI Agent 在 ISSUE_02 开发过程中自动提交代码。**

### 权限范围

AI Agent 仅可对以下文件进行 commit：

| 目录/文件 | 说明 |
|-----------|------|
| `src/FairWorkly.Application/Payroll/Services/ComplianceEngine/*` | 规则引擎实现 |
| `src/FairWorkly.Application/DependencyInjection.cs` | DI 注册 |
| `tests/FairWorkly.UnitTests/Unit/*RuleTests.cs` | 规则单元测试 |
| `tests/FairWorkly.UnitTests/TestData/Csv/*` | 测试数据 |

### Commit 规则

| 规则 | 要求 |
|------|------|
| **语言** | Commit message 必须使用 **English** |
| **格式** | Conventional Commits (`feat:`, `test:`, `fix:`, `chore:`) |
| **粒度** | 按逻辑单元提交（一个功能点 = 代码 + 测试） |
| **测试** | 提交前必须运行 `dotnet test` 确保通过 |
| **Push** | ❌ 禁止 push，仅 commit 到本地 |
| **确认** | 按规则自动提交，无需每次确认 |
| **签名** | ❌ 禁止添加 AI 生成标识（如 "Generated with Claude Code"、Co-Authored-By 等） |

### Commit 顺序建议

```
1. feat(compliance): add IComplianceRule interface and RateTableProvider
2. feat(compliance): implement BaseRateRule with unit tests
3. feat(compliance): implement PenaltyRateRule with unit tests
4. feat(compliance): implement CasualLoadingRule with unit tests
5. feat(compliance): implement SuperannuationRule with unit tests
6. chore(compliance): register ComplianceEngine services in DI
```

### 权限终止

当 ISSUE_02 所有验收标准完成后，此 commit 权限自动失效。

---

## Pre-Validation (前置数据校验)

> ⚠️ **架构说明**：根据 [ARCHITECTURE.md](../../.raw_materials/TECH_CONSTRAINTS/ARCHITECTURE.md)，Pre-Validation 是 **Handler 的职责**，不是 ComplianceEngine 的一部分。
>
> Pre-Validation 的具体实现在 **ISSUE_03 (ValidatePayrollHandler)** 中，详见 [ISSUE_03_Handler_API.md](./ISSUE_03_Handler_API.md)。

ComplianceEngine 仅负责接收**已通过前置校验**的 Payslip 实体，执行 4 个合规规则检查。

---

## 四个规则概述

| 规则 | 检查目标 | 适用对象 | Severity |
|------|----------|----------|----------|
| **BaseRateRule** | 时薪是否达到法定最低标准 | 所有员工 | CRITICAL / WARNING |
| **PenaltyRateRule** | 周末/公休日罚金倍率是否正确 | 有周末/公休工时的员工 | ERROR |
| **CasualLoadingRule** | Casual 员工是否获得 25% Loading | 仅 Casual 员工 | CRITICAL / WARNING |
| **SuperannuationRule** | 养老金是否按 12% 缴纳 | 所有有收入的员工 | ERROR / WARNING |

---

## 技术要点

### IComplianceRule 接口

```csharp
public interface IComplianceRule
{
    string RuleName { get; }

    List<PayrollIssue> Evaluate(Payslip payslip, Guid organizationId);
}
```

### RateTableProvider（静态费率表）

```csharp
public static class RateTableProvider
{
    // 基础费率 (Permanent Rate)
    public static readonly Dictionary<int, decimal> PermanentRates = new()
    {
        { 1, 26.55m },
        { 2, 27.16m },
        { 3, 27.58m },
        { 4, 28.12m },
        { 5, 29.27m },
        { 6, 29.70m },
        { 7, 31.19m },
        { 8, 32.45m }
    };

    // Casual Rate (含 25% Loading)
    public static readonly Dictionary<int, decimal> CasualRates = new()
    {
        { 1, 33.19m },
        { 2, 33.95m },
        { 3, 34.48m },
        { 4, 35.15m },
        { 5, 36.59m },
        { 6, 37.13m },
        { 7, 38.99m },
        { 8, 40.56m }
    };

    // 罚金倍率
    public static class Multipliers
    {
        public static class Permanent
        {
            public const decimal Saturday = 1.25m;
            public const decimal Sunday = 1.50m;
            public const decimal PublicHoliday = 2.25m;
        }

        public static class Casual
        {
            public const decimal Saturday = 1.50m;
            public const decimal Sunday = 1.75m;
            public const decimal PublicHoliday = 2.50m;
        }
    }

    // 养老金率
    public const decimal SuperannuationRate = 0.12m;
}
```

### 容差常量

```csharp
// 时薪比对容差：$0.01
public const decimal RateTolerance = 0.01m;

// 金额比对容差：$0.05
public const decimal PayTolerance = 0.05m;
```

### 输出规则

- 规则通过时 **不输出** PayrollIssue（不输出 INFO 级别）
- 仅在违规时输出 PayrollIssue（WARNING / ERROR / CRITICAL）
- 统计摘要记录在 `PayrollValidation` 层（TotalChecks, PassedChecks, FailedChecks）

---

## 规则详细逻辑

### BaseRateRule

```
1. 获取该等级的 Permanent Rate（无论员工类型）
2. 计算实际时薪 = OrdinaryPay / OrdinaryHours
3. 如果 OrdinaryHours = 0，跳过
4. 如果 实际时薪 < Permanent Rate - 0.01：
   → CRITICAL，记录差额
5. 如果 CSV Hourly Rate < Permanent Rate - 0.01：
   → WARNING（档案配置错误）
6. 否则 → 无问题
```

### PenaltyRateRule

> ⚠️ **重要**：即使 EmploymentType 是 Casual，计算基数也必须使用 **Permanent Rate**（不是 Casual Rate）！倍率则根据 EmploymentType 查表。

```
对 Saturday / Sunday / PublicHoliday 分别检查：
1. 如果对应 Hours = 0，跳过
2. 获取该等级的 Permanent Rate（所有员工类型都用这个作为基数）
3. 根据 EmploymentType 获取对应倍率（Permanent 或 Casual 倍率表）
4. 计算应发金额 = Permanent Rate × 倍率 × Hours
5. 如果实际 Pay < 应发金额 - 0.05：
   → ERROR，记录差额
6. 否则 → 无问题
```

### CasualLoadingRule

```
1. 如果不是 Casual，跳过
2. 获取该等级的 Casual Rate（含 25% Loading）
3. 计算实际时薪 = OrdinaryPay / OrdinaryHours
4. 如果 OrdinaryHours = 0，跳过
5. 如果 实际时薪 < Casual Rate - 0.01：
   → CRITICAL，记录差额
6. 如果 CSV Hourly Rate < Casual Rate - 0.01：
   → WARNING（档案配置错误）
7. 否则 → 无问题
```

### SuperannuationRule

```
1. 计算 AnyWorkHours = OrdinaryHours + SaturdayHours + SundayHours + PublicHolidayHours
2. 如果 Gross Pay <= 0：
   a. 如果 AnyWorkHours > 0：
      → WARNING ("Missing Gross Pay Data")
      → 结束流程
   b. 否则（无工时也无工资）：
      → 跳过（无薪周期）
3. 计算应缴养老金 = Gross Pay × 12%
4. 如果 Superannuation Paid < 应缴 - 0.05：
   → ERROR，记录差额
5. 否则 → 无问题
```

---

## PayrollIssue 字段填充指南

| 字段 | 说明 | 示例 |
|------|------|------|
| CheckType | 规则名称 | "Base Rate Check" |
| Severity | 严重程度 | IssueSeverity.Critical |
| Description | 简短描述 | "Base Rate below minimum" |
| ExpectedValue | 法定标准值 | 26.55 |
| ActualValue | 实际支付值 | 23.50 |
| AffectedUnits | 涉及单位数 | 40.0 (小时) |
| UnitType | 单位类型 | "Hour" 或 "Currency" |
| ContextLabel | 上下文标签 | "Retail Award Level 2" |

---

## 验收标准

- [x] BaseRateRule 能正确识别时薪低于最低标准的违规
- [x] PenaltyRateRule 能正确计算三种场景（Sat/Sun/PH）的罚金差额
- [x] CasualLoadingRule 仅对 Casual 员工生效
- [x] SuperannuationRule 能正确计算 12% 养老金差额
- [x] 所有规则的 Severity 判定正确
- [x] 返回的 PayrollIssue 包含完整的 Evidence 数据
- [x] 服务已在 DependencyInjection.cs 中注册
- [x] 单元测试通过 (TC-BASE, TC-PENALTY, TC-CASUAL, TC-SUPER)

---

## 对应测试

### 单元测试用例

| 测试用例 ID | CSV 文件 | 验证目标 | 预期 Severity |
|-------------|----------|----------|---------------|
| TC-BASE-001 | TEST_04_BaseRate_AllPass.csv | 基础费率合规 | 无输出 |
| TC-BASE-002 | TEST_05_BaseRate_Violations.csv (行2-5) | 基础费率违规 | CRITICAL |
| TC-BASE-003 | TEST_05_BaseRate_Violations.csv (行6) | 系统费率配置错误 | WARNING |
| TC-PENALTY-001 | TEST_06_Saturday_Violations.csv | 周六罚金违规 | ERROR |
| TC-PENALTY-002 | TEST_07_Sunday_Violations.csv | 周日罚金违规 | ERROR |
| TC-PENALTY-003 | TEST_08_PublicHoliday_Violations.csv | 公休罚金违规 | ERROR |
| TC-CASUAL-001 | TEST_09_Casual_AllPass.csv | Casual Loading 合规 | 无输出 |
| TC-CASUAL-002 | TEST_10_Casual_Violations.csv (行2-3) | Casual Loading 违规 | CRITICAL |
| TC-CASUAL-003 | TEST_10_Casual_Violations.csv (行4) | 系统费率配置错误 | WARNING |
| TC-CASUAL-004 | TEST_10_Casual_Violations.csv (行5) | 非 Casual 员工跳过 | 无输出 |
| TC-SUPER-001 | TEST_11_Super_AllPass.csv | 养老金合规 | 无输出 |
| TC-SUPER-002 | TEST_12_Super_Violations.csv (行2-5) | 养老金违规 | ERROR |
| TC-SUPER-003 | TEST_12_Super_Violations.csv (行6) | 缺少 Gross Pay 但有工时 | WARNING |
| TC-PREVAL-001 | TEST_17_PreValidation.csv | 必填字段缺失 | WARNING + Skip |

### 边界值测试 (TEST_16_EdgeCases.csv)

| 测试用例 ID | Employee ID | 场景 | 预期结果 |
|-------------|-------------|------|----------|
| TC-EDGE-001 | EDGE001 | 恰好达到最低费率 | PASS |
| TC-EDGE-002 | EDGE002 | 在容差边界 ($26.54 vs $26.55) | PASS (容差内) |
| TC-EDGE-003 | EDGE003 | 零工时员工 | SKIP |
| TC-EDGE-006 | EDGE006 | 养老金在容差内 ($0.04 差额) | PASS |
| TC-EDGE-007 | EDGE007 | 罚金刚超容差 ($0.06 差额) | ERROR |
| TC-EDGE-008 | EDGE008 | Level 8 Casual 最高等级 | PASS |

### 集成测试

| CSV 文件 | 场景 | 说明 |
|----------|------|------|
| TEST_13_AllCompliant.csv | 全部合规 | 无 PayrollIssue 输出 |
| TEST_14_AllViolations.csv | 全部违规 | 多种违规组合 |
| TEST_15_MixedScenarios.csv | 混合场景 | 部分合规部分违规 |
| TEST_16_EdgeCases.csv | 边界值 | 容差边界测试 |

---

## 依赖

- **前置 Issue**: ISSUE_01（需要解析后的薪资数据）
- **Entity**: `Payslip`, `PayrollIssue`（已存在，不可修改）
