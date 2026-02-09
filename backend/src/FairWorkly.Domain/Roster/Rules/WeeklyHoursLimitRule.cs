using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Roster.Entities;
using FairWorkly.Domain.Roster.Parameters;
using FairWorkly.Domain.Roster.Enums;
using FairWorkly.Domain.Roster.ValueObjects;

namespace FairWorkly.Domain.Roster.Rules;

/// <summary>
/// Validates total weekly hours against award limits
/// - Full-time: exceeds 38 hours triggers info (overtime)
/// - Part-time: exceeds GuaranteedHours triggers warning
/// - Casual: no limit (skipped)
/// </summary>
public class WeeklyHoursLimitRule(IRosterRuleParametersProvider parametersProvider)
    : IRosterComplianceRule
{
    private readonly IRosterRuleParametersProvider _parametersProvider = parametersProvider;

    public RosterCheckType CheckType => RosterCheckType.WeeklyHoursLimit;

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

            var employmentType = employee.EmploymentType;

            // Casual employees have no weekly hours limit under Retail Award
            if (employmentType == EmploymentType.Casual)
                continue;

            var parameters = _parametersProvider.Get(employee.AwardType);

            var weeks = employeeShifts.GroupBy(shift => GetWeekStart(shift.Date));
            foreach (var week in weeks)
            {
                var totalHours = week.Sum(shift => shift.NetHours);
                var weekDates = week.Select(shift => shift.Date);

                // Determine threshold based on employment type
                decimal threshold;
                bool exceedsLimit;

                if (employmentType == EmploymentType.PartTime)
                {
                    // Part-time: use GuaranteedHours as threshold
                    if (employee.GuaranteedHours == null || employee.GuaranteedHours <= 0)
                    {
                        // Report missing GuaranteedHours as data quality issue
                        issues.Add(
                            new RosterIssue
                            {
                                OrganizationId = employeeShifts[0].OrganizationId,
                                RosterValidationId = validationId,
                                ShiftId = null,
                                EmployeeId = employeeId,
                                CheckType = RosterCheckType.DataQuality,
                                Severity = IssueSeverity.Warning,
                                Description = "Part-time employee missing GuaranteedHours - weekly hours limit cannot be validated",
                                AffectedDates = AffectedDateSet.FromDates(weekDates),
                                AffectedShiftsCount = week.Count(),
                            }
                        );
                        continue;
                    }

                    threshold = employee.GuaranteedHours.Value;
                    exceedsLimit = totalHours > threshold;
                }
                else
                {
                    // Full-time/FixedTerm: use standard 38 hour limit
                    threshold = parameters.WeeklyHoursLimit;
                    exceedsLimit = totalHours > threshold;
                }

                if (!exceedsLimit)
                    continue;

                issues.Add(
                    new RosterIssue
                    {
                        OrganizationId = employeeShifts[0].OrganizationId,
                        RosterValidationId = validationId,
                        ShiftId = null,
                        EmployeeId = employeeId,
                        CheckType = CheckType,
                        Severity = employmentType == EmploymentType.PartTime
                            ? IssueSeverity.Warning
                            : IssueSeverity.Info,
                        Description = employmentType == EmploymentType.PartTime
                            ? $"Total weekly hours {totalHours:F2} exceed guaranteed {threshold:F0} hours"
                            : $"Total weekly hours {totalHours:F2} exceed {threshold:F0} hour limit",
                        ExpectedValue = threshold,
                        ActualValue = totalHours,
                        AffectedDates = AffectedDateSet.FromDates(weekDates),
                        AffectedShiftsCount = week.Count(),
                    }
                );
            }
        }

        return issues;
    }

    private static DateTime GetWeekStart(DateTime date)
    {
        var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.Date.AddDays(-diff);
    }
}
