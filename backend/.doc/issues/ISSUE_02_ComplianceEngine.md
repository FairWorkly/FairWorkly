# ISSUE_02: 合规规则引擎

> ⚠️ **当前任务**：此 Issue 是下一个开发目标

## 目标

实现 4 个薪资合规检查规则引擎。

**一句话**：检查每条薪资记录是否符合 Retail Award 的费率规定，识别违规并计算差额。

---

## 状态

- [ ] 开发中 ← **当前位置**
- [ ] 测试通过
- [ ] Review 完成

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

```
对 Saturday / Sunday / PublicHoliday 分别检查：
1. 如果对应 Hours = 0，跳过
2. 根据 EmploymentType 获取倍率
3. 计算应发金额 = Permanent Rate × 倍率 × Hours
4. 如果实际 Pay < 应发金额 - 0.05：
   → ERROR，记录差额
5. 否则 → 无问题
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
1. 如果 Gross Pay = 0，跳过
2. 计算应缴养老金 = Gross Pay × 12%
3. 如果 Superannuation Paid < 应缴 - 0.05：
   → ERROR，记录差额
4. 否则 → 无问题
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

- [ ] BaseRateRule 能正确识别时薪低于最低标准的违规
- [ ] PenaltyRateRule 能正确计算三种场景（Sat/Sun/PH）的罚金差额
- [ ] CasualLoadingRule 仅对 Casual 员工生效
- [ ] SuperannuationRule 能正确计算 12% 养老金差额
- [ ] 所有规则的 Severity 判定正确
- [ ] 返回的 PayrollIssue 包含完整的 Evidence 数据
- [ ] 服务已在 DependencyInjection.cs 中注册
- [ ] 单元测试通过 (TC-BASE, TC-PENALTY, TC-CASUAL, TC-SUPER)

---

## 对应测试

| 测试用例 | CSV 文件 | 验证目标 |
|----------|----------|----------|
| TC-BASE-001 | TEST_04_BaseRate_AllPass.csv | 基础费率合规 |
| TC-BASE-002 | TEST_05_BaseRate_Violations.csv | 基础费率违规 |
| TC-PENALTY-001 | TEST_06_Saturday_Violations.csv | 周六罚金违规 |
| TC-PENALTY-002 | TEST_07_Sunday_Violations.csv | 周日罚金违规 |
| TC-PENALTY-003 | TEST_08_PublicHoliday_Violations.csv | 公休罚金违规 |
| TC-CASUAL-001 | TEST_09_Casual_AllPass.csv | Casual Loading 合规 |
| TC-CASUAL-002 | TEST_10_Casual_Violations.csv | Casual Loading 违规 |
| TC-SUPER-001 | TEST_11_Super_AllPass.csv | 养老金合规 |
| TC-SUPER-002 | TEST_12_Super_Violations.csv | 养老金违规 |

---

## 依赖

- **前置 Issue**: ISSUE_01（需要解析后的薪资数据）
- **Entity**: `Payslip`, `PayrollIssue`（已存在，不可修改）
