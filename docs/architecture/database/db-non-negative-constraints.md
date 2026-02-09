# Database Non-Negative Constraints Contract

> **Purpose**: Define which fields can safely have `CHECK >= 0` constraints and which must allow negative values for business reasons.

## Summary

| Category            | Can Add CHECK >= 0 | Reason                                              |
| ------------------- | ------------------ | --------------------------------------------------- |
| Statistics/Counts   | ✅ Yes             | Counts cannot be negative                           |
| Durations/Hours     | ✅ Yes             | Time durations cannot be negative                   |
| Award Configuration | ✅ Yes             | Configuration values are non-negative               |
| Payroll Amounts     | ❌ No              | Correction/reversal entries require negative values |
| Issue Values        | ❌ No              | Variance calculations may be negative               |

---

## ✅ Safe to Add Non-Negative CHECK

### Shift

| Field                | Type | Constraint                                                      |
| -------------------- | ---- | --------------------------------------------------------------- |
| `MealBreakDuration`  | int? | `CHECK (MealBreakDuration IS NULL OR MealBreakDuration >= 0)`   |
| `RestBreaksDuration` | int? | `CHECK (RestBreaksDuration IS NULL OR RestBreaksDuration >= 0)` |

### Roster

| Field            | Type    | Constraint                                     |
| ---------------- | ------- | ---------------------------------------------- |
| `TotalShifts`    | int     | `CHECK (TotalShifts >= 0)`                     |
| `TotalHours`     | decimal | `CHECK (TotalHours >= 0)`                      |
| `TotalEmployees` | int     | `CHECK (TotalEmployees >= 0)`                  |
| `WeekNumber`     | int     | `CHECK (WeekNumber >= 1 AND WeekNumber <= 53)` |
| `Year`           | int     | `CHECK (Year >= 2000)`                         |

### RosterValidation

| Field                 | Type | Constraint                         |
| --------------------- | ---- | ---------------------------------- |
| `TotalShifts`         | int  | `CHECK (TotalShifts >= 0)`         |
| `PassedShifts`        | int  | `CHECK (PassedShifts >= 0)`        |
| `FailedShifts`        | int  | `CHECK (FailedShifts >= 0)`        |
| `TotalIssuesCount`    | int  | `CHECK (TotalIssuesCount >= 0)`    |
| `CriticalIssuesCount` | int  | `CHECK (CriticalIssuesCount >= 0)` |
| `AffectedEmployees`   | int  | `CHECK (AffectedEmployees >= 0)`   |

### PayrollValidation

| Field                 | Type | Constraint                         |
| --------------------- | ---- | ---------------------------------- |
| `TotalPayslips`       | int  | `CHECK (TotalPayslips >= 0)`       |
| `PassedCount`         | int  | `CHECK (PassedCount >= 0)`         |
| `FailedCount`         | int  | `CHECK (FailedCount >= 0)`         |
| `TotalIssuesCount`    | int  | `CHECK (TotalIssuesCount >= 0)`    |
| `CriticalIssuesCount` | int  | `CHECK (CriticalIssuesCount >= 0)` |

### Award

| Field                      | Type    | Constraint                              |
| -------------------------- | ------- | --------------------------------------- |
| `SaturdayPenaltyRate`      | decimal | `CHECK (SaturdayPenaltyRate >= 0)`      |
| `SundayPenaltyRate`        | decimal | `CHECK (SundayPenaltyRate >= 0)`        |
| `PublicHolidayPenaltyRate` | decimal | `CHECK (PublicHolidayPenaltyRate >= 0)` |
| `CasualLoadingRate`        | decimal | `CHECK (CasualLoadingRate >= 0)`        |
| `MinimumShiftHours`        | decimal | `CHECK (MinimumShiftHours >= 0)`        |
| `MaxConsecutiveDays`       | int     | `CHECK (MaxConsecutiveDays >= 0)`       |
| `MealBreakThresholdHours`  | int     | `CHECK (MealBreakThresholdHours >= 0)`  |
| `MealBreakMinutes`         | int     | `CHECK (MealBreakMinutes >= 0)`         |
| `MinimumRestPeriodHours`   | int     | `CHECK (MinimumRestPeriodHours >= 0)`   |
| `OrdinaryWeeklyHours`      | int     | `CHECK (OrdinaryWeeklyHours >= 0)`      |

### AwardLevel

| Field                | Type    | Constraint                        |
| -------------------- | ------- | --------------------------------- |
| `LevelNumber`        | int     | `CHECK (LevelNumber >= 1)`        |
| `FullTimeHourlyRate` | decimal | `CHECK (FullTimeHourlyRate >= 0)` |
| `PartTimeHourlyRate` | decimal | `CHECK (PartTimeHourlyRate >= 0)` |
| `CasualHourlyRate`   | decimal | `CHECK (CasualHourlyRate >= 0)`   |

### Employee

| Field              | Type | Constraint                                                |
| ------------------ | ---- | --------------------------------------------------------- |
| `GuaranteedHours`  | int? | `CHECK (GuaranteedHours IS NULL OR GuaranteedHours >= 0)` |
| `AwardLevelNumber` | int  | `CHECK (AwardLevelNumber >= 1)`                           |

### Organization

| Field                  | Type | Constraint                          |
| ---------------------- | ---- | ----------------------------------- |
| `CurrentEmployeeCount` | int  | `CHECK (CurrentEmployeeCount >= 0)` |

### OrganizationAward

| Field           | Type | Constraint                   |
| --------------- | ---- | ---------------------------- |
| `EmployeeCount` | int  | `CHECK (EmployeeCount >= 0)` |

### Payslip (Hours and Rate Only)

> **Note**: In Australian payroll practice, corrections are typically done via negative **amounts**, not negative **hours**. Hours and rates remain non-negative; only the pay amounts can be negative for reversals.

| Field                | Type     | Constraint                                                  |
| -------------------- | -------- | ----------------------------------------------------------- |
| `HourlyRate`         | decimal  | `CHECK (HourlyRate >= 0)`                                   |
| `OrdinaryHours`      | decimal  | `CHECK (OrdinaryHours >= 0)`                                |
| `SaturdayHours`      | decimal  | `CHECK (SaturdayHours >= 0)`                                |
| `SundayHours`        | decimal  | `CHECK (SundayHours >= 0)`                                  |
| `PublicHolidayHours` | decimal  | `CHECK (PublicHolidayHours >= 0)`                           |
| `OvertimeHours`      | decimal? | `CHECK (OvertimeHours IS NULL OR OvertimeHours >= 0)`       |

---

## ❌ Must NOT Add Non-Negative CHECK

### Payslip (Correction/Reversal Scenario)

**Reason**: Payroll systems allow negative amounts for correction entries (reversals, adjustments). This is explicitly tested and supported in the codebase.

**Evidence**:

- `CsvParserServiceTests.cs:256, 279` - Negative OrdinaryPay/GrossPay parsing succeeds
- `PenaltyRateRuleTests.cs:219+` - Negative penalty pay treated as correction (Warning)
- `BaseRateRuleTests.cs:213+` - Negative OrdinaryPay returns Warning
- `SuperannuationRuleTests.cs:181+` - Negative GrossPay returns Warning

| Field              | Type     | Why No Constraint         |
| ------------------ | -------- | ------------------------- |
| `OrdinaryPay`      | decimal  | Correction entries        |
| `SaturdayPay`      | decimal  | Correction entries        |
| `SundayPay`        | decimal  | Correction entries        |
| `PublicHolidayPay` | decimal  | Correction entries        |
| `OvertimePay`      | decimal? | Correction entries        |
| `CasualLoadingPay` | decimal? | Correction entries        |
| `Allowances`       | decimal? | Correction entries        |
| `GrossPay`         | decimal  | Correction entries        |
| `Tax`              | decimal  | Refunds possible          |
| `Superannuation`   | decimal  | Adjustments               |
| `OtherDeductions`  | decimal? | Refunds                   |
| `NetPay`           | decimal  | Net result of corrections |

### PayrollIssue

| Field           | Type     | Why No Constraint                   |
| --------------- | -------- | ----------------------------------- |
| `ExpectedValue` | decimal? | Variance can be negative            |
| `ActualValue`   | decimal? | Reflects payslip (can be negative)  |
| `AffectedUnits` | decimal? | Could represent negative adjustment |

### RosterIssue

| Field           | Type     | Why No Constraint             |
| --------------- | -------- | ----------------------------- |
| `ExpectedValue` | decimal? | Consistency with PayrollIssue |
| `ActualValue`   | decimal? | Consistency with PayrollIssue |

---

## Implementation Notes

### EF Core Migration Example

```csharp
migrationBuilder.Sql(@"
    ALTER TABLE Shifts ADD CONSTRAINT CHK_Shift_MealBreakDuration
        CHECK (MealBreakDuration IS NULL OR MealBreakDuration >= 0);

    ALTER TABLE Shifts ADD CONSTRAINT CHK_Shift_RestBreaksDuration
        CHECK (RestBreaksDuration IS NULL OR RestBreaksDuration >= 0);
");
```

### Naming Convention

`CHK_{TableName}_{FieldName}`

Example: `CHK_Shift_MealBreakDuration`

---

## Change Log

| Date       | Author | Change                                                                 |
| ---------- | ------ | ---------------------------------------------------------------------- |
| 2026-02-06 | -      | Initial contract created                                               |
| 2026-02-06 | -      | Confirmed: Payslip Hours/Rate fields safe (AU practice: only amounts negative) |
