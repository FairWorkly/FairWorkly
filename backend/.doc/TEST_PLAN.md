# TEST_PLAN - 测试方案

> **Payroll 模块的测试策略和用例清单。**

---

## 1. 测试框架

| 组件 | 技术 |
|------|------|
| 测试框架 | xUnit |
| 断言库 | FluentAssertions |
| Mock | Moq |
| 内存数据库 | Microsoft.EntityFrameworkCore.InMemory |

---

## 2. 测试环境

| 测试类型 | 数据库 | 说明 |
|----------|--------|------|
| 单元测试 | InMemory | 快速、隔离 |
| 集成测试 | 本地 PostgreSQL | 真实数据库行为 |

### 数据库操作命令

```bash
# 清库重建（集成测试前执行）
dotnet ef database drop --force --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API
dotnet ef database update --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API
```

---

## 3. 测试数据

测试 CSV 文件位于：`tests/FairWorkly.UnitTests/TestData/Csv/`

### 3.1 员工同步测试 (TEST_01 ~ TEST_03)

| 文件 | 员工数 | 测试目标 |
|------|--------|----------|
| `TEST_01_NewEmployees.csv` | 5 | 创建新员工 |
| `TEST_02_UpdateEmployees.csv` | 3 | 更新已有员工 |
| `TEST_03_MixedEmployees.csv` | 6 | 混合新旧 |

### 3.2 合规规则测试 (TEST_04 ~ TEST_12)

| 文件 | 规则 | 预期结果 |
|------|------|----------|
| `TEST_04_BaseRate_AllPass.csv` | BaseRate | 0 问题 |
| `TEST_05_BaseRate_Violations.csv` | BaseRate | 4 CRITICAL |
| `TEST_06_Saturday_Violations.csv` | PenaltyRate | 3 ERROR |
| `TEST_07_Sunday_Violations.csv` | PenaltyRate | 3 ERROR |
| `TEST_08_PublicHoliday_Violations.csv` | PenaltyRate | 3 ERROR |
| `TEST_09_Casual_AllPass.csv` | CasualLoading | 0 问题 |
| `TEST_10_Casual_Violations.csv` | CasualLoading | 混合 |
| `TEST_11_Super_AllPass.csv` | Super | 0 问题 |
| `TEST_12_Super_Violations.csv` | Super | 4 ERROR |

### 3.3 综合测试 (TEST_13 ~ TEST_16)

| 文件 | 员工数 | 场景 |
|------|--------|------|
| `TEST_13_AllCompliant.csv` | 10 | 全部合规 |
| `TEST_14_AllViolations.csv` | 8 | 全部违规 |
| `TEST_15_MixedScenarios.csv` | 12 | 真实混合 |
| `TEST_16_EdgeCases.csv` | 8 | 边界值 |

---

## 4. 测试用例详情

### 4.1 员工同步测试

#### TC-SYNC-001: 创建新员工

**输入**: `TEST_01_NewEmployees.csv`
**前置**: Employee 表无 NEW001~NEW005
**预期**:
```csharp
employees.Count.Should().Be(5);
employees.Should().Contain(e => e.EmployeeNumber == "NEW001");
```

#### TC-SYNC-002: 更新已有员工

**输入**: `TEST_02_UpdateEmployees.csv`
**前置**: 预插入 EMP001~EMP003（旧数据）
**预期**:
```csharp
employees.Count.Should().Be(3);  // 数量不变
employees.Single(e => e.EmployeeNumber == "EMP001")
    .AwardLevelNumber.Should().Be(2);  // 从 1 变 2
```

#### TC-SYNC-003: 混合场景

**输入**: `TEST_03_MixedEmployees.csv`
**前置**: 预插入 MIX001~MIX003
**预期**:
```csharp
employees.Count.Should().Be(6);  // 3 更新 + 3 新建
```

---

### 4.1.1 CsvParser 错误场景（待补充）

| ID | 测试方法 | 场景 | 输入 | 预期输出 |
|----|----------|------|------|----------|
| CSV-ERR-001 | `ParseAsync_MissingEmployeeId_ReturnsError` | Employee ID 缺失 | `"",Alice,...` | Error: "Employee ID is required" |
| CSV-ERR-002 | `ParseAsync_InvalidDateFormat_ReturnsError` | 日期格式错误 | `15/12/2025` (非 ISO) | Error: "Invalid date format" |
| CSV-ERR-003 | `ParseAsync_InvalidEmploymentType_ReturnsError` | 无效 EmploymentType | `Contract` | Error: "Invalid Employment Type" |
| CSV-ERR-004 | `ParseAsync_EmptyFile_ReturnsError` | 空文件 | 0 bytes | Error: "File is empty" |
| CSV-ERR-005 | `ParseAsync_HeaderOnly_ReturnsEmptyRows` | 只有表头 | Header row only | rows = [], errors = [] |

---

### 4.2 基础费率测试

#### TC-BASE-001: 基础费率合规

**输入**: `TEST_04_BaseRate_AllPass.csv`
**预期**:
```csharp
result.Summary.TotalIssues.Should().Be(0);
result.Summary.PassedCount.Should().Be(4);
```

#### TC-BASE-002: 基础费率违规

**输入**: `TEST_05_BaseRate_Violations.csv`
**预期**:
```csharp
result.Issues.Should().HaveCount(4);
result.Issues.Should().AllSatisfy(i => i.Severity.Should().Be(4));  // CRITICAL

var viol001 = result.Issues.Single(i => i.EmployeeId == "VIOL001");
viol001.ImpactAmount.Should().BeApproximately(62.00m, 0.05m);
```

#### 4.2.1 BaseRateRule Level 边界测试（待补充）

| ID | 测试方法 | Level | Permanent Rate | 预期 |
|----|----------|-------|----------------|------|
| BASE-LVL-004a | `Evaluate_Level4_WhenAtMinimum_ShouldReturnNoIssues` | Level 4 | $28.12 | Pass |
| BASE-LVL-004b | `Evaluate_Level4_WhenBelowMinimum_ShouldReturnCritical` | Level 4 | < $28.12 | CRITICAL |
| BASE-LVL-006 | `Evaluate_Level6_WhenBelowMinimum_ShouldReturnCritical` | Level 6 | < $29.70 | CRITICAL |
| BASE-LVL-007 | `Evaluate_Level7_WhenAtMinimum_ShouldReturnNoIssues` | Level 7 | $31.19 | Pass |
| BASE-LVL-008 | `Evaluate_Level8_WhenBelowMinimum_ShouldReturnCritical` | Level 8 | < $32.45 | CRITICAL |

**费率参考** (from Payroll_Engine_Logic.md):

| Level | Permanent Rate |
|-------|----------------|
| Level 4 | $28.12 |
| Level 5 | $29.27 |
| Level 6 | $29.70 |
| Level 7 | $31.19 |
| Level 8 | $32.45 |

---

### 4.3 罚金费率测试

#### TC-PENALTY-001: 周六罚金违规

**输入**: `TEST_06_Saturday_Violations.csv`
**预期**:
```csharp
result.Issues.Should().HaveCount(3);
result.Issues.Should().AllSatisfy(i => i.Severity.Should().Be(3));  // ERROR
result.Issues.Should().AllSatisfy(i =>
    i.Description.ContextLabel.Should().Contain("Saturday"));
```

#### TC-PENALTY-002: 周日罚金违规

**输入**: `TEST_07_Sunday_Violations.csv`
**预期**: 3 个 ERROR，ContextLabel 含 "Sunday"

#### TC-PENALTY-003: 公休日违规

**输入**: `TEST_08_PublicHoliday_Violations.csv`
**预期**: 3 个 ERROR，ContextLabel 含 "Public Holiday"

---

### 4.4 Casual Loading 测试

#### TC-CASUAL-001: Casual Loading 合规

**输入**: `TEST_09_Casual_AllPass.csv`
**预期**:
```csharp
var casualIssues = result.Issues.Where(i => i.CategoryType == "CasualLoading");
casualIssues.Should().BeEmpty();
```

#### TC-CASUAL-002: Casual Loading 违规

**输入**: `TEST_10_Casual_Violations.csv`
**预期**:
```csharp
var issues = result.Issues.Where(i => i.CategoryType == "CasualLoading").ToList();
issues.Should().HaveCount(3);
issues.Single(i => i.EmployeeId == "CASVIOL001").Severity.Should().Be(4);  // CRITICAL
issues.Single(i => i.EmployeeId == "CASVIOL003").Severity.Should().Be(2);  // WARNING
```

---

### 4.5 养老金测试

#### TC-SUPER-001: 养老金合规

**输入**: `TEST_11_Super_AllPass.csv`
**预期**: Superannuation 类别无问题

#### TC-SUPER-002: 养老金违规

**输入**: `TEST_12_Super_Violations.csv`
**预期**:
```csharp
result.Issues.Where(i => i.CategoryType == "Superannuation")
    .Should().HaveCount(4);
```

---

### 4.6 综合测试

#### TC-INT-001: 全部合规

**输入**: `TEST_13_AllCompliant.csv`
**预期**:
```csharp
result.Status.Should().Be("Passed");
result.Summary.PassedCount.Should().Be(10);
result.Summary.TotalIssues.Should().Be(0);
```

#### TC-INT-002: 全部违规

**输入**: `TEST_14_AllViolations.csv`
**预期**:
```csharp
result.Status.Should().Be("Failed");
result.Summary.AffectedEmployees.Should().Be(8);
result.Categories.Should().HaveCount(4);  // 四种问题都有
```

#### TC-INT-003: 混合场景

**输入**: `TEST_15_MixedScenarios.csv`
**预期**:
```csharp
result.Summary.PassedCount.Should().Be(6);
result.Summary.AffectedEmployees.Should().Be(6);
```

#### TC-INT-004: 边界值

**输入**: `TEST_16_EdgeCases.csv`

| 员工 | 场景 | 预期 |
|------|------|------|
| EDGE001 | 恰好 $26.55 | Pass |
| EDGE002 | $26.54 (差 1 分) | CRITICAL |
| EDGE003 | 0 工时 | 跳过 |
| EDGE005 | 0 毛工资 | 跳过 Super |
| EDGE006 | Super 差 $0.04 | Pass (在容差内) |
| EDGE007 | Penalty 差 $0.06 | ERROR (超容差) |

---

## 5. Issue 与测试对应

| Issue | 测试文件 | 测试用例 |
|-------|----------|----------|
| ISSUE_01 | CsvParserServiceTests.cs, EmployeeSyncServiceTests.cs | TC-SYNC-001~003 |
| ISSUE_02 | BaseRateRuleTests.cs, PenaltyRateRuleTests.cs, CasualLoadingRuleTests.cs, SuperannuationRuleTests.cs | TC-BASE-001~002, TC-PENALTY-001~003, TC-CASUAL-001~002, TC-SUPER-001~002 |
| ISSUE_03 | PayrollValidationTests.cs | TC-INT-001~004 |

---

## 6. 开发测试流程

```
1. 开发 CsvParserService
   └── 写单元测试 → 通过

2. 开发 EmployeeSyncService
   └── 用 TEST_01~03 测试 → 通过

3. 开发 BaseRateRule
   └── 用 TEST_04~05 测试 → 通过

4. 开发其他规则...
   └── 用对应 CSV 测试 → 通过

5. 开发 Handler (串联)
   └── 用 TEST_13~16 集成测试 → 通过

6. 暴露 Controller
   └── Swagger 手测 → 完成
```

---

## 7. Swagger 手动测试

### 步骤

1. 启动后端: `dotnet run --project src/FairWorkly.API`
2. 打开: `https://localhost:5001/swagger`
3. 找到 `POST /api/payroll/validation`
4. 上传 CSV，填写参数:
   - awardType: `Retail`
   - payPeriod: `Weekly`
   - weekStarting: `2025-12-15`
   - weekEnding: `2025-12-21`
   - state: `VIC`
5. 执行，检查响应

### 快速验证清单

- [ ] `TEST_13` → status: "Passed", totalIssues: 0
- [ ] `TEST_05` → 4 个 CRITICAL，CategoryType: "BaseRate"
- [ ] `TEST_12` → 4 个 ERROR，CategoryType: "Superannuation"

---

## 8. 测试统计

### 8.1 测试文件汇总

| 测试文件 | 测试数 | 状态 |
|----------|--------|------|
| CsvParserServiceTests.cs | 11 | ✅ |
| EmployeeSyncServiceTests.cs | 6 | ✅ |
| EmployeeSyncIntegrationTests.cs | 3 | ✅ |
| BaseRateRuleTests.cs | 17 | ✅ |
| PenaltyRateRuleTests.cs | 13 | ✅ |
| CasualLoadingRuleTests.cs | 17 | ✅ |
| SuperannuationRuleTests.cs | 22 | ✅ |
| **总计** | **89** | ✅ |

### 8.2 业务规则覆盖

| 规则 | Severity 覆盖 | 状态 |
|------|---------------|------|
| BaseRate | CRITICAL ✅, WARNING ✅ | 完整 |
| PenaltyRate | ERROR ✅ (Sat/Sun/PH) | 完整 |
| CasualLoading | CRITICAL ✅, WARNING ✅ | 完整 |
| Superannuation | ERROR ✅, WARNING ✅ | 完整 |

### 8.3 数值一致性

所有测试数据与 `Payroll_Engine_Logic.md` 一致：

| 参数 | 文档值 | 测试值 | 状态 |
|------|--------|--------|------|
| Level 1 Permanent | $26.55 | $26.55 | ✅ |
| Level 2 Permanent | $27.16 | $27.16 | ✅ |
| Casual Loading | 25% | 25% | ✅ |
| Superannuation | 12% | 12% | ✅ |
| Rate Tolerance | $0.01 | $0.01 | ✅ |
| Pay Tolerance | $0.05 | $0.05 | ✅ |
