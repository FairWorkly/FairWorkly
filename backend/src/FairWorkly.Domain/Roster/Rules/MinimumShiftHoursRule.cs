using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Roster.Entities;
using FairWorkly.Domain.Roster.Parameters;
using FairWorkly.Domain.Roster.Enums;

namespace FairWorkly.Domain.Roster.Rules;

/// <summary>
/// Validates minimum shift duration requirements by employment type
/// </summary>
public class MinimumShiftHoursRule(IRosterRuleParametersProvider parametersProvider)
    : IRosterComplianceRule
{
    private readonly IRosterRuleParametersProvider _parametersProvider = parametersProvider;

    public RosterCheckType CheckType => RosterCheckType.MinimumShiftHours;

    public List<RosterIssue> Evaluate(IEnumerable<Shift> shifts, Guid validationId)
    {
        var issues = new List<RosterIssue>();

        foreach (var shift in shifts)
        {
            if (shift.Employee == null)
                continue;

            var parameters = _parametersProvider.Get(shift.Employee.AwardType);
            var minHours = parameters.GetMinShiftHours(shift.Employee.EmploymentType);
            if (shift.Duration >= minHours)
                continue;

            issues.Add(
                new RosterIssue
                {
                    OrganizationId = shift.OrganizationId,
                    RosterValidationId = validationId,
                    ShiftId = shift.Id,
                    EmployeeId = shift.EmployeeId,
                    CheckType = CheckType,
                    Severity = IssueSeverity.Error,
                    Description =
                        $"Shift only {shift.Duration:F2} hours, minimum is {minHours:F2} hours",
                    ExpectedValue = minHours,
                    ActualValue = shift.Duration,
                }
            );
        }

        return issues;
    }
}
