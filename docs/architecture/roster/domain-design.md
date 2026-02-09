# Roster Domain Layer Design Document

> This document helps new developers quickly understand the Roster compliance checking module design.

## 1. Business Background

### 1.1 What is a Roster?

A Roster is a work schedule that employers create for their employees, typically organized by week. In Australia, rosters must comply with **Modern Awards** regulations, or employers may face penalties from Fair Work.

### 1.2 What is a Modern Award?

A Modern Award is an industry minimum standard set by Australia's Fair Work Commission, covering:
- Minimum wages
- Working hour limits
- Rest period requirements
- Overtime rules, etc.

FairWorkly currently supports 3 common Awards:

| Award | Code | Industry |
|-------|------|----------|
| General Retail Industry Award 2020 | MA000004 | Retail |
| Hospitality Industry Award 2020 | MA000009 | Hospitality |
| Clerks—Private Sector Award 2020 | MA000002 | Office/Clerical |

### 1.3 What Compliance Checks Do We Perform?

We currently implement **5 roster compliance checks** (plus 1 data quality check):

| Check | Code | Description |
|-------|------|-------------|
| Data Quality | DataQuality | Validates data integrity (e.g., Employee loaded) |
| Minimum Shift Hours | MinimumShiftHours | Shifts cannot be too short |
| Meal Break | MealBreak | Long shifts must have meal breaks |
| Rest Period Between Shifts | RestPeriodBetweenShifts | Sufficient rest between consecutive shifts |
| Weekly Hours Limit | WeeklyHoursLimit | Cannot exceed weekly hour limits |
| Maximum Consecutive Days | MaximumConsecutiveDays | Cannot work too many days in a row |

---

## 2. Award Differences Comparison

### 2.1 Parameter Differences Table

| Check | Retail | Hospitality | Clerks |
|-------|--------|-------------|--------|
| **Min Shift (Part-time/Casual)** | 3 hours | 3 hours | 3 hours |
| **Meal Break Threshold** | >5 hours | >5 hours | >5 hours |
| **Standard Rest Period** | 12 hours | 10 hours | 10 hours |
| **Reduced Rest (by agreement)** | 10 hours | 8 hours | 10 hours (no reduction) |
| **Weekly Hours Limit** | 38 hours | 38 hours | 38 hours |
| **Max Consecutive Days** | 6 days | 7 days | 5 days |

### 2.2 Understanding the Differences

#### Rest Period Between Shifts
- **Retail**: Most strict - 12 hours standard, can reduce to 10 hours with written agreement
- **Hospitality**: Industry-specific (many night shifts) - 10 hours standard, can reduce to 8 hours
- **Clerks**: Standard office - 10 hours, no reduction option

#### Consecutive Working Days
- **Retail**: Maximum 6 days, must rest on day 7
- **Hospitality**: Allows 7 days (industry demand), but needs proper arrangement
- **Clerks**: Maximum 5 days, matches standard Monday-Friday work pattern

---

## 3. Code Architecture

### 3.1 Directory Structure

```
FairWorkly.Domain/
└── Roster/
    ├── Entities/           # Entities
    │   ├── Roster.cs       # Weekly roster
    │   ├── Shift.cs        # Individual shift
    │   ├── RosterValidation.cs  # Validation record
    │   └── RosterIssue.cs  # Issue record
    ├── Enums/
    │   └── RosterCheckType.cs   # Check type enum
    ├── Parameters/         # Award parameters
    │   ├── IRosterRuleParametersProvider.cs
    │   ├── RosterRuleParameterSet.cs
    │   └── AwardRosterRuleParametersProvider.cs
    ├── Rules/              # Compliance rules (Strategy Pattern)
    │   ├── IRosterComplianceRule.cs
    │   ├── DataQualityRule.cs
    │   ├── MinimumShiftHoursRule.cs
    │   ├── MealBreakRule.cs
    │   ├── RestPeriodRule.cs
    │   ├── WeeklyHoursLimitRule.cs
    │   └── ConsecutiveDaysRule.cs
    └── ValueObjects/
        └── AffectedDateSet.cs   # Affected dates collection
```

### 3.2 Core Class Relationships

```
┌─────────────────────────────────────────────────────────────────┐
│                    IRosterComplianceRule                         │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐                │
│  │DataQuality  │ │MinShiftHours│ │ MealBreak   │ ...            │
│  └─────────────┘ └─────────────┘ └─────────────┘                │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼ depends on
┌─────────────────────────────────────────────────────────────────┐
│              IRosterRuleParametersProvider                       │
│                              │                                   │
│                              ▼                                   │
│              AwardRosterRuleParametersProvider                   │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐                │
│  │RetailAward  │ │Hospitality  │ │ClerksAward  │                │
│  │Parameters   │ │Parameters   │ │Parameters   │                │
│  └─────────────┘ └─────────────┘ └─────────────┘                │
└─────────────────────────────────────────────────────────────────┘
```

---

## 4. Key Design Decisions

### 4.1 Strategy Pattern

Each compliance rule is an independent class implementing `IRosterComplianceRule`:

```csharp
public interface IRosterComplianceRule
{
    RosterCheckType CheckType { get; }
    List<RosterIssue> Evaluate(IEnumerable<Shift> shifts, Guid validationId);
}
```

**Benefits**:
- Adding new rules only requires a new class, no changes to existing code
- Each rule can be tested independently
- Rules can be enabled/disabled as needed

### 4.2 Award Parameterization

Different Award parameters are encapsulated in `RosterRuleParameterSet`:

```csharp
public sealed record RosterRuleParameterSet(
    decimal MinShiftHoursPartTime,      // Minimum shift duration
    int StandardRestPeriodHours,         // Standard rest period
    int ReducedRestPeriodHours,          // Reduced rest by agreement
    int MaxConsecutiveDays,              // Maximum consecutive days
    // ... more parameters
);
```

Rules obtain the current employee's Award parameters via `IRosterRuleParametersProvider`:

```csharp
var parameters = _parametersProvider.Get(employee.AwardType);
var minHours = parameters.GetMinShiftHours(employee.EmploymentType);
```

### 4.3 Issue Severity Levels

| Level | Meaning | Example |
|-------|---------|---------|
| **Error** | Clear violation, must fix | Less than 8 hours rest between shifts |
| **Warning** | Needs attention, may need written agreement | 10 hours rest (below 12 hour standard) |
| **Info** | Informational | Full-time employee over 38 hours this week (overtime notice) |

### 4.4 Reserved Extension Fields

These fields are defined in entities but not yet used by rules (reserved for future):

| Field | Location | Future Use |
|-------|----------|------------|
| `Employee.IsStudent` | Employee.cs | Student work hour limits |
| `Shift.IsOnCall` | Shift.cs | On-call allowance calculations |
| `Shift.IsPublicHoliday` | Shift.cs | Public holiday rostering restrictions |

---

## 5. Rule Execution Flow

```
1. Application layer calls ValidateRosterHandler
                │
                ▼
2. Load Roster + Shifts + Employees (Include navigation properties)
                │
                ▼
3. Iterate through all IRosterComplianceRule instances
   ┌────────────────────────────────────────┐
   │ foreach (var rule in _rules)           │
   │ {                                       │
   │     var issues = rule.Evaluate(shifts);│
   │     allIssues.AddRange(issues);        │
   │ }                                       │
   └────────────────────────────────────────┘
                │
                ▼
4. Save RosterValidation + RosterIssues to database
                │
                ▼
5. Return validation results to frontend for display
```

---

## 6. How to Extend

### 6.1 Adding a New Compliance Rule

1. Create a new class in `Roster/Rules/`:

```csharp
public class OvertimeRule : IRosterComplianceRule
{
    public RosterCheckType CheckType => RosterCheckType.Overtime;

    public List<RosterIssue> Evaluate(IEnumerable<Shift> shifts, Guid validationId)
    {
        // Implement overtime check logic
    }
}
```

2. Add a new value to the `RosterCheckType` enum:

```csharp
public enum RosterCheckType
{
    // ... existing values
    Overtime = 7,
}
```

3. Register in DI container:

```csharp
services.AddScoped<IRosterComplianceRule, OvertimeRule>();
```

### 6.2 Supporting a New Award

1. Add to `AwardType` enum:

```csharp
public enum AwardType
{
    // ... existing values
    FastFoodIndustryAward2020 = 4,
}
```

2. Add parameters in `AwardRosterRuleParametersProvider`:

```csharp
private static readonly RosterRuleParameterSet FastFoodAward = new(
    MinShiftHoursPartTime: 3m,
    StandardRestPeriodHours: 10,
    // ... other parameters
);

public RosterRuleParameterSet Get(AwardType awardType)
{
    return awardType switch
    {
        // ... existing mappings
        AwardType.FastFoodIndustryAward2020 => FastFoodAward,
        _ => throw new NotSupportedException(...),
    };
}
```

### 6.3 Adding New Parameter Dimensions

If you need to differentiate parameters by Employment Type or Classification Level:

```csharp
// Add method to RosterRuleParameterSet
public int GetMaxConsecutiveDays(EmploymentType type)
{
    return type switch
    {
        EmploymentType.Casual => MaxConsecutiveDays + 1,  // Casual can work one more day
        _ => MaxConsecutiveDays,
    };
}
```

---

## 7. Required Background Knowledge

### 7.1 Technical Concepts

| Concept | Description |
|---------|-------------|
| **DDD (Domain-Driven Design)** | Entities, Value Objects, Domain Services |
| **Strategy Pattern** | Each rule is a strategy, replaceable and composable |
| **Record Types** | C# 9+ immutable data types, used for parameter sets |
| **EF Core Navigation Properties** | `Shift.Employee` requires `.Include()` to load |

### 7.2 Business Concepts

| Concept | Description |
|---------|-------------|
| **Modern Award** | Australian industry minimum standards set by Fair Work |
| **Employment Type** | FullTime / PartTime / Casual / FixedTerm |
| **Overnight Shift** | Cross-midnight shift, e.g., 22:00 - 06:00 |
| **Reduced Rest Period** | Shortened rest allowed by written agreement |

### 7.3 Recommended Reading

- [Fair Work - Breaks](https://www.fairwork.gov.au/employment-conditions/hours-of-work-breaks-and-rosters/breaks)
- [Retail Award Summary](https://www.fairwork.gov.au/employment-conditions/awards/award-summary/ma000004-summary)
- [Hospitality Award Summary](https://www.fairwork.gov.au/employment-conditions/awards/award-summary/ma000009-summary)

---

## 8. FAQ

### Q: Why do rules skip when Employee is null?

A: This is by design. `DataQualityRule` detects and reports missing Employee data, so other rules can safely `continue`. DataQualityRule already produces an Error-level Issue for this case.

### Q: Why does Shift.NetHours return 0 when negative?

A: Defensive programming. If break duration exceeds shift duration, `DataQualityRule` already reports this issue. Clipping NetHours to 0 prevents negative number anomalies in downstream calculations.

### Q: Why is MealBreakTable identical across all three Awards?

A: Fair Work currently specifies the same meal break rules for these three Awards. The parameterized design allows easy adaptation if any Award changes in the future.

### Q: Does Payroll compliance checking also support these three Awards?

A: Currently Payroll only supports Retail Award (rate table in `RateTableProvider`). Roster and Payroll Award support are independent and can be expanded incrementally.

---

## 9. Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | 2026-02 | Initial version, 3 Awards + 6 checks |

---

*Document Maintenance: Please update this file when changes are made*
