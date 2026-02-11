using FairWorkly.Domain.Common.Enums;

namespace FairWorkly.Domain.Roster.Parameters;

/// <summary>
/// Provides Award-specific roster rule parameters for compliance checks.
/// Each Award has different requirements for rest periods, consecutive days, etc.
/// </summary>
public sealed class AwardRosterRuleParametersProvider : IRosterRuleParametersProvider
{
    /// <summary>
    /// General Retail Industry Award 2020 (MA000004)
    /// - Rest Period: 12h standard, 10h by agreement
    /// - Max Consecutive Days: 6
    /// </summary>
    private static readonly RosterRuleParameterSet RetailAward =
        new(
            MinShiftHoursFullTime: 0m,
            MinShiftHoursPartTime: 3m,
            MinShiftHoursCasual: 3m,
            MealBreakThresholdHours: 5m,
            // MealBreakTable: (minHours, maxHours, breakMinutes)
            // Shift duration > minHours and <= maxHours requires breakMinutes
            MealBreakTable:
            [
                (5m, 6m, 30),   // >5h to 6h → 30 min
                (6m, 7m, 30),   // >6h to 7h → 30 min
                (7m, 8m, 30),   // >7h to 8h → 30 min
                (8m, 9m, 30),   // >8h to 9h → 30 min
                (9m, 10m, 60),  // >9h to 10h → 60 min
                (10m, 24m, 60), // >10h → 60 min
            ],
            StandardRestPeriodHours: 12,
            ReducedRestPeriodHours: 10,
            WeeklyHoursLimit: 38m,
            MaxConsecutiveDays: 6
        );

    /// <summary>
    /// Hospitality Industry (General) Award 2020 (MA000009)
    /// - Rest Period: 10h standard, 8h for roster changeover
    /// - Max Consecutive Days: 7 (more flexible than Retail)
    /// </summary>
    private static readonly RosterRuleParameterSet HospitalityAward =
        new(
            MinShiftHoursFullTime: 0m,
            MinShiftHoursPartTime: 3m,
            MinShiftHoursCasual: 3m,
            MealBreakThresholdHours: 5m,
            // Same meal break table as Retail Award
            MealBreakTable:
            [
                (5m, 6m, 30),
                (6m, 7m, 30),
                (7m, 8m, 30),
                (8m, 9m, 30),
                (9m, 10m, 60),
                (10m, 24m, 60),
            ],
            StandardRestPeriodHours: 10,
            ReducedRestPeriodHours: 8,
            WeeklyHoursLimit: 38m,
            MaxConsecutiveDays: 7
        );

    /// <summary>
    /// Clerks—Private Sector Award 2020 (MA000002)
    /// - Rest Period: 10h standard
    /// - Max Consecutive Days: 5 (standard office week)
    /// </summary>
    private static readonly RosterRuleParameterSet ClerksAward =
        new(
            MinShiftHoursFullTime: 0m,
            MinShiftHoursPartTime: 3m,
            MinShiftHoursCasual: 3m,
            MealBreakThresholdHours: 5m,
            // Same meal break table as Retail Award
            MealBreakTable:
            [
                (5m, 6m, 30),
                (6m, 7m, 30),
                (7m, 8m, 30),
                (8m, 9m, 30),
                (9m, 10m, 60),
                (10m, 24m, 60),
            ],
            StandardRestPeriodHours: 10,
            ReducedRestPeriodHours: 10,
            WeeklyHoursLimit: 38m,
            MaxConsecutiveDays: 5
        );

    public RosterRuleParameterSet Get(AwardType awardType)
    {
        return awardType switch
        {
            AwardType.GeneralRetailIndustryAward2020 => RetailAward,
            AwardType.HospitalityIndustryAward2020 => HospitalityAward,
            AwardType.ClerksPrivateSectorAward2020 => ClerksAward,
            _ => throw new NotSupportedException(
                $"Award type '{awardType}' is not supported for roster compliance checks. " +
                $"Please add parameters for this award in {nameof(AwardRosterRuleParametersProvider)}."),
        };
    }
}

