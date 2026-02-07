using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Roster.Entities;
using FairWorkly.Domain.Roster.Parameters;
using FairWorkly.Domain.Roster.Enums;

namespace FairWorkly.Domain.Roster.Rules;

/// <summary>
/// Validates meal break requirements for shifts over threshold
/// </summary>
public class MealBreakRule(IRosterRuleParametersProvider parametersProvider) : IRosterComplianceRule
{
    private readonly IRosterRuleParametersProvider _parametersProvider = parametersProvider;

    public RosterCheckType CheckType => RosterCheckType.MealBreak;

    public List<RosterIssue> Evaluate(IEnumerable<Shift> shifts, Guid validationId)
    {
        var issues = new List<RosterIssue>();

        foreach (var shift in shifts)
        {
            if (shift.Employee == null)
                continue;

            var parameters = _parametersProvider.Get(shift.Employee.AwardType);
            var requiredBreak = parameters.GetRequiredMealBreakMinutes(shift.Duration);
            if (requiredBreak <= 0)
                continue;

            if (!shift.HasMealBreak)
            {
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
                            $"No meal break provided for {shift.Duration:F2} hour shift",
                        ExpectedValue = requiredBreak,
                        ActualValue = 0,
                    }
                );
                continue;
            }

            var actualBreak = shift.MealBreakDuration ?? 0;
            if (actualBreak < requiredBreak)
            {
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
                            $"Meal break only {actualBreak} minutes, required {requiredBreak} minutes",
                        ExpectedValue = requiredBreak,
                        ActualValue = actualBreak,
                    }
                );
            }
        }

        return issues;
    }
}
