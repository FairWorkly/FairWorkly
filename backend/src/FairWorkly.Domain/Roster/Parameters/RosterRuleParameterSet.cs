using FairWorkly.Domain.Common.Enums;

namespace FairWorkly.Domain.Roster.Parameters;

/// <summary>
/// Award-specific parameters for roster compliance rules.
/// </summary>
/// <param name="MinShiftHoursFullTime">Minimum shift duration for full-time employees (hours)</param>
/// <param name="MinShiftHoursPartTime">Minimum shift duration for part-time employees (hours)</param>
/// <param name="MinShiftHoursCasual">Minimum shift duration for casual employees (hours)</param>
/// <param name="MealBreakThresholdHours">Shifts longer than this require a meal break (hours)</param>
/// <param name="MealBreakTable">
/// Meal break requirements by shift duration.
/// Each tuple: (minHours, maxHours, breakMinutes) where shift duration > minHours and &lt;= maxHours requires breakMinutes.
/// Example: (5, 6, 30) means shifts over 5h up to 6h require 30 minute break.
/// </param>
/// <param name="StandardRestPeriodHours">Standard minimum rest between shifts (hours)</param>
/// <param name="ReducedRestPeriodHours">Reduced rest period allowed by written agreement (hours)</param>
/// <param name="WeeklyHoursLimit">Maximum ordinary hours per week</param>
/// <param name="MaxConsecutiveDays">Maximum consecutive working days before required rest day</param>
public sealed record RosterRuleParameterSet(
    decimal MinShiftHoursFullTime,
    decimal MinShiftHoursPartTime,
    decimal MinShiftHoursCasual,
    decimal MealBreakThresholdHours,
    (decimal minHours, decimal maxHours, int breakMinutes)[] MealBreakTable,
    int StandardRestPeriodHours,
    int ReducedRestPeriodHours,
    decimal WeeklyHoursLimit,
    int MaxConsecutiveDays
)
{
    public decimal GetMinShiftHours(EmploymentType employmentType)
    {
        return employmentType switch
        {
            EmploymentType.FullTime => MinShiftHoursFullTime,
            EmploymentType.PartTime => MinShiftHoursPartTime,
            EmploymentType.Casual => MinShiftHoursCasual,
            EmploymentType.FixedTerm => MinShiftHoursFullTime,
            _ => MinShiftHoursCasual,
        };
    }

    public int GetRequiredMealBreakMinutes(decimal shiftHours)
    {
        if (shiftHours <= MealBreakThresholdHours)
            return 0;

        foreach (var (minHours, maxHours, breakMinutes) in MealBreakTable)
        {
            if (shiftHours > minHours && shiftHours <= maxHours)
                return breakMinutes;
        }

        return 60;
    }
}

