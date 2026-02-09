using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Roster.Entities;
using FairWorkly.Domain.Roster.ValueObjects;
using FairWorkly.Domain.Roster.Parameters;
using FairWorkly.Domain.Roster.Enums;

namespace FairWorkly.Domain.Roster.Rules;

/// <summary>
/// Validates maximum consecutive working days
/// </summary>
public class ConsecutiveDaysRule(IRosterRuleParametersProvider parametersProvider)
    : IRosterComplianceRule
{
    private readonly IRosterRuleParametersProvider _parametersProvider = parametersProvider;

    public RosterCheckType CheckType => RosterCheckType.MaximumConsecutiveDays;

    public List<RosterIssue> Evaluate(IEnumerable<Shift> shifts, Guid validationId)
    {
        var issues = new List<RosterIssue>();

        var shiftsByEmployee = shifts
            .GroupBy(shift => shift.EmployeeId)
            .ToDictionary(group => group.Key, group => group.ToList());

        foreach (var (employeeId, employeeShifts) in shiftsByEmployee)
        {
            if (employeeShifts.Count == 0)
                continue;

            var employee = employeeShifts[0].Employee;
            if (employee == null)
                continue;

            var parameters = _parametersProvider.Get(employee.AwardType);

            var workDates = employeeShifts
                .Select(shift => shift.Date.Date)
                .Distinct()
                .OrderBy(date => date)
                .ToList();

            var streakStartIndex = 0;
            for (var i = 1; i <= workDates.Count; i++)
            {
                var isConsecutive =
                    i < workDates.Count && workDates[i] == workDates[i - 1].AddDays(1);

                if (isConsecutive)
                    continue;

                var streakLength = i - streakStartIndex;
                if (streakLength > parameters.MaxConsecutiveDays)
                {
                    var streakDates = workDates.Skip(streakStartIndex).Take(streakLength);

                    issues.Add(
                        new RosterIssue
                        {
                            OrganizationId = employeeShifts[0].OrganizationId,
                            RosterValidationId = validationId,
                            ShiftId = null,
                            EmployeeId = employeeId,
                            CheckType = CheckType,
                            Severity = IssueSeverity.Warning,
                            Description =
                                $"Worked {streakLength} consecutive days, maximum is {parameters.MaxConsecutiveDays}",
                            ExpectedValue = parameters.MaxConsecutiveDays,
                            ActualValue = streakLength,
                            AffectedDates = AffectedDateSet.FromDates(streakDates),
                            AffectedShiftsCount = streakLength,
                        }
                    );
                }

                streakStartIndex = i;
            }
        }

        return issues;
    }
}
