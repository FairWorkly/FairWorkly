using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Roster.Entities;
using FairWorkly.Domain.Roster.Enums;

namespace FairWorkly.Domain.Roster.Rules;

/// <summary>
/// Validates data quality issues in roster shifts.
/// - Missing Employee navigation property (prevents other rules from running)
/// - Break duration exceeds shift duration
/// </summary>
public class DataQualityRule : IRosterComplianceRule
{
    public RosterCheckType CheckType => RosterCheckType.DataQuality;

    public List<RosterIssue> Evaluate(IEnumerable<Shift> shifts, Guid validationId)
    {
        var issues = new List<RosterIssue>();

        // Track employees with missing data to avoid duplicate issues
        var employeesWithMissingData = new HashSet<Guid>();

        foreach (var shift in shifts)
        {
            // Check for missing Employee navigation property
            // This prevents other compliance rules from evaluating this employee
            if (shift.Employee == null && !employeesWithMissingData.Contains(shift.EmployeeId))
            {
                employeesWithMissingData.Add(shift.EmployeeId);
                issues.Add(
                    new RosterIssue
                    {
                        OrganizationId = shift.OrganizationId,
                        RosterValidationId = validationId,
                        ShiftId = shift.Id,
                        EmployeeId = shift.EmployeeId,
                        CheckType = CheckType,
                        Severity = IssueSeverity.Error,
                        Description = "Employee data not loaded - compliance rules cannot be evaluated for this employee",
                    }
                );
                continue;
            }

            // Check for break duration exceeding shift duration
            var totalBreakMinutes = (shift.MealBreakDuration ?? 0) + (shift.RestBreaksDuration ?? 0);
            if (totalBreakMinutes <= 0)
                continue;

            var shiftDurationMinutes = shift.Duration * 60m;
            if (shiftDurationMinutes <= 0)
                continue;

            if (totalBreakMinutes <= shiftDurationMinutes)
                continue;

            issues.Add(
                new RosterIssue
                {
                    OrganizationId = shift.OrganizationId,
                    RosterValidationId = validationId,
                    ShiftId = shift.Id,
                    EmployeeId = shift.EmployeeId,
                    CheckType = CheckType,
                    Severity = IssueSeverity.Warning,
                    Description =
                        $"Total break minutes {totalBreakMinutes} exceed shift duration minutes {shiftDurationMinutes:F0}",
                    ExpectedValue = shiftDurationMinutes,
                    ActualValue = totalBreakMinutes,
                }
            );
        }

        return issues;
    }
}
