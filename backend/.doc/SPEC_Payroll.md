# SPEC_Payroll - Payroll 模块技术规格

> **Payroll 模块的详细技术规格。包含业务规则、数据结构、流程设计。**

---

## 1. 业务目标

验证用户上传的薪资 CSV 是否符合澳大利亚 Retail Award (MA000004) 的规定。

**核心流程**：
```
CSV上传 → 解析数据 → 员工Upsert → 4规则检查 → 输出违规报告
```

---

## 2. CSV 输入规格

### 2.1 字段定义

| 字段名称 | 必填 | 类型 | 说明 |
|----------|------|------|------|
| Employee ID | ✅ | string | 员工工号，用于 Upsert |
| Employee Name | ✅ | string | 员工姓名 |
| Pay Period Start | ✅ | Date | 格式 `YYYY-MM-DD` |
| Pay Period End | ✅ | Date | 格式 `YYYY-MM-DD` |
| Award Type | ✅ | string | 必须包含 "Retail" 或 "MA000004" |
| Classification | ✅ | string | 如 `Level 1` |
| Employment Type | ✅ | Enum | FullTime/PartTime/Casual/FixedTerm |
| Hourly Rate | ✅ | decimal | 档案时薪 |
| Ordinary Hours | ✅ | decimal | 基础工时 |
| Ordinary Pay | ✅ | decimal | 基础工资 |
| Saturday Hours | ❌ | decimal | 默认 0 |
| Saturday Pay | ❌ | decimal | 周六工资 |
| Sunday Hours | ❌ | decimal | 默认 0 |
| Sunday Pay | ❌ | decimal | 周日工资 |
| Public Holiday Hours | ❌ | decimal | 默认 0 |
| Public Holiday Pay | ❌ | decimal | 公休工资 |
| Gross Pay | ✅ | decimal | 税前总额 |
| Superannuation Paid | ✅ | decimal | 实缴养老金 |

---

## 3. 费率表（静态配置）

### 3.1 基础费率 (生效日期: 01/07/2025)

| 级别 | Permanent Rate (基数) | Casual Rate (含25% Loading) |
|------|----------------------|----------------------------|
| Level 1 | $26.55 | $33.19 |
| Level 2 | $27.16 | $33.95 |
| Level 3 | $27.58 | $34.48 |
| Level 4 | $28.12 | $35.15 |
| Level 5 | $29.27 | $36.59 |
| Level 6 | $29.70 | $37.13 |
| Level 7 | $31.19 | $38.99 |
| Level 8 | $32.45 | $40.56 |

### 3.2 罚金倍率

| 场景 | Permanent 倍率 | Casual 倍率 |
|------|---------------|-------------|
| Saturday (周六) | 1.25x (125%) | 1.50x (150%) |
| Sunday (周日) | 1.50x (150%) | 1.75x (175%) |
| Public Holiday | 2.25x (225%) | 2.50x (250%) |

**重要**：所有倍率计算均以 **Permanent Rate** 为基数。

### 3.3 养老金

| 参数 | 数值 | 说明 |
|------|------|------|
| Super Guarantee | 12% (0.12) | 计算基数为 Gross Pay |

---

## 4. 合规规则

### 4.1 前置校验 (Pre-Validation)

> ⚠️ **架构说明**：Pre-Validation 在 **ValidatePayrollHandler** 中实现，不是 ComplianceEngine 的一部分。
> 详见 [ARCHITECTURE.md](../.raw_materials/TECH_CONSTRAINTS/ARCHITECTURE.md)。

在执行任何规则之前，检查必填字段是否完整。

**逻辑**：
```
如果必填字段缺失或无法解析：
    → 判定 WARNING
    → 跳过该员工的所有规则检查
```

### 4.2 规则 1: 基础费率 (Base Rate Check)

**目标**：确保时薪不低于法定最低标准（Permanent Rate）

**适用对象**：所有员工

**逻辑**：
```
1. 获取该等级的 Permanent Rate（不管员工是否 Casual）
2. 计算实际时薪 = OrdinaryPay / OrdinaryHours
3. 如果实际时薪 < Permanent Rate - 0.01：
   → CRITICAL（实发低于最低工资）
4. 如果 CSV 中的 Hourly Rate < Permanent Rate - 0.01：
   → WARNING（档案配置错误）
5. 否则 → PASS
```

### 4.3 规则 2: 罚金费率 (Penalty Rate Check)

**目标**：确保周末/公休日的罚金倍率正确

**适用对象**：有周末/公休工时的员工

**逻辑**（以 Saturday 为例）：
```
1. 如果 Saturday Hours = 0，跳过
2. 获取 Permanent Rate 和对应倍率
3. 计算应发金额 = Permanent Rate × 倍率 × Saturday Hours
4. 如果 Saturday Pay < 应发金额 - 0.05：
   → ERROR（罚金未足额支付）
5. 否则 → PASS
```

Sunday 和 Public Holiday 同理。

### 4.4 规则 3: Casual Loading (Casual Loading Check)

**目标**：确保 Casual 员工获得 25% Loading

**适用对象**：仅 Casual 员工

**逻辑**：
```
1. 如果不是 Casual，跳过
2. 获取该等级的 Casual Rate（含 25% Loading）
3. 计算实际时薪 = OrdinaryPay / OrdinaryHours
4. 如果实际时薪 < Casual Rate - 0.01：
   → CRITICAL（Loading 未支付）
5. 如果 CSV 中的 Hourly Rate < Casual Rate - 0.01：
   → WARNING（档案配置错误）
6. 否则 → PASS
```

### 4.5 规则 4: 养老金 (Superannuation Check)

**目标**：确保养老金按 12% 缴纳

**适用对象**：所有有收入的员工

**逻辑**：
```
1. 如果 Gross Pay = 0，跳过
2. 计算应缴养老金 = Gross Pay × 12%
3. 如果 Superannuation Paid < 应缴 - 0.05：
   → ERROR（养老金欠缴）
4. 否则 → PASS
```

---

## 5. Severity 判定标准

| 等级 | 数值 | 触发条件 | 业务含义 |
|------|------|----------|----------|
| **CRITICAL** | 4 | 实发时薪 < 法定最低 | 触犯法律红线 |
| **ERROR** | 3 | 基础达标但未支付额外权益 | 合规义务未履行 |
| **WARNING** | 2 | 档案配置错误 / 数据缺失 | 潜在风险 |
| **INFO** | 1 | 审计通过 | 合规 |

---

## 6. API 输出规格

### 6.1 请求

```
POST /api/payroll/validation
Content-Type: multipart/form-data
```

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| file | File | ✅ | CSV 文件 (Max: 10MB) |
| awardType | string | ✅ | Retail / Hospitality / Clerks |
| payPeriod | string | ✅ | Weekly / Fortnightly / Monthly |
| weekStarting | string | ✅ | YYYY-MM-DD |
| weekEnding | string | ✅ | YYYY-MM-DD |
| state | string | ✅ | VIC / NSW / QLD / ... |
| enableBaseRateCheck | bool | ❌ | 默认 true |
| enablePenaltyCheck | bool | ❌ | 默认 true |
| enableCasualLoadingCheck | bool | ❌ | 默认 true |
| enableSuperCheck | bool | ❌ | 默认 true |

### 6.2 响应（v1.3）

> **v1.3 更新**：`evidence` → `description`，新增 `warning` 字段，`categories.key` 新增 `PreValidation`

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
        "key": "PreValidation | BaseRate | PenaltyRate | Superannuation | CasualLoading",
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
        "description": {
          "actualValue": 23.50,
          "expectedValue": 25.41,
          "affectedUnits": 40.0,
          "unitType": "Hour | Currency",
          "contextLabel": "Retail Award Level 2"
        },
        "warning": null
      }
    ]
  }
}
```

---

## 7. 数据流向图

```
┌──────────────┐
│   CSV File   │
└──────┬───────┘
       │
       ▼
┌──────────────────┐
│  CsvParserService │ → List<PayrollCsvRow>
└──────┬───────────┘
       │
       ▼
┌────────────────────┐
│ EmployeeSyncService │ → Dict<EmployeeNumber, EmployeeId>
└──────┬─────────────┘
       │
       ▼
┌──────────────────────────┐
│  Create Payslip Records  │
└──────┬───────────────────┘
       │
       ▼
┌──────────────────────────────────┐
│  ⭐ Pre-Validation (Handler)    │
│  检查必填字段完整性              │
│  缺失 → WARNING + 跳过规则检查   │
└──────┬───────────────────────────┘
       │
       ▼
┌──────────────────────────────────┐
│       ComplianceEngine           │
│  ┌─────────────┐ ┌─────────────┐ │
│  │ BaseRateRule│ │PenaltyRule  │ │
│  └─────────────┘ └─────────────┘ │
│  ┌─────────────┐ ┌─────────────┐ │
│  │CasualLoading│ │SuperRule    │ │
│  └─────────────┘ └─────────────┘ │
└──────┬───────────────────────────┘
       │
       ▼
┌──────────────────────────┐
│  Create PayrollIssue     │
│  Records                 │
└──────┬───────────────────┘
       │
       ▼
┌──────────────────────────┐
│  Build ValidationResult  │
│  DTO & Return            │
└──────────────────────────┘
```

---

## 8. 雇佣类型映射

| CSV / Enum 类型 | 算法逻辑组 | 说明 |
|-----------------|-----------|------|
| Casual | Casual | 使用 Casual Rate + Casual 倍率 |
| FullTime | Permanent | 使用 Permanent Rate + Permanent 倍率 |
| PartTime | Permanent | 同上 |
| FixedTerm | Permanent | 同上（不享受 Casual Loading）|
