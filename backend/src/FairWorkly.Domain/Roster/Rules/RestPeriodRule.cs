using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Roster.Entities;
using FairWorkly.Domain.Roster.Parameters;
using FairWorkly.Domain.Roster.Enums;
using FairWorkly.Domain.Roster.ValueObjects;

namespace FairWorkly.Domain.Roster.Rules;

/// <summary>
/// Validates minimum rest period between consecutive shifts
/// </summary>
public class RestPeriodRule(IRosterRuleParametersProvider parametersProvider) : IRosterComplianceRule
{
    private readonly IRosterRuleParametersProvider _parametersProvider = parametersProvider;

    public RosterCheckType CheckType => RosterCheckType.RestPeriodBetweenShifts;

    public List<RosterIssue> Evaluate(IEnumerable<Shift> shifts, Guid validationId)
    {
        var issues = new List<RosterIssue>();
        var shiftsByEmployee = shifts
            .GroupBy(shift => shift.EmployeeId)
            .ToDictionary(
                group => group.Key,
                group => group.OrderBy(shift => shift.StartDateTime).ToList()
            );

        foreach (var (employeeId, employeeShifts) in shiftsByEmployee)
        {
            var employee = employeeShifts.FirstOrDefault()?.Employee;
            if (employee == null)
                continue;

            var parameters = _parametersProvider.Get(employee.AwardType);

            for (var i = 0; i < employeeShifts.Count - 1; i++)
            {
                var currentShift = employeeShifts[i];
                var nextShift = employeeShifts[i + 1];
                var restHours = (nextShift.StartDateTime - currentShift.EndDateTime).TotalHours;

                if (restHours >= parameters.StandardRestPeriodHours)
                    continue;

                var minimumAllowedRestHours = restHours < parameters.ReducedRestPeriodHours
                    ? parameters.ReducedRestPeriodHours
                    : parameters.StandardRestPeriodHours;

                issues.Add(
                    new RosterIssue
                    {
                        OrganizationId = currentShift.OrganizationId,
                        RosterValidationId = validationId,
                        ShiftId = null,
                        EmployeeId = employeeId,
                        CheckType = CheckType,
                        Severity = restHours < parameters.ReducedRestPeriodHours
                            ? IssueSeverity.Error
                            : IssueSeverity.Warning,
                        Description =
                            $"Only {restHours:F2} hours rest between shifts, minimum is {minimumAllowedRestHours} hours",
                        ExpectedValue = minimumAllowedRestHours,
                        ActualValue = (decimal)restHours,
                        AffectedDates = AffectedDateSet.FromDates([currentShift.Date, nextShift.Date]),
                        AffectedShiftsCount = 2,
                    }
                );
            }
        }

        return issues;
    }
}
