namespace FairWorkly.Domain.Roster.Enums;

/// <summary>
/// Types of compliance checks performed on roster/shifts
/// Used for consistent categorization and aggregation of issues
///
/// PERSISTENCE CONTRACT:
/// - This enum is persisted to the database as a string (enum name) via EF Core conversion (HasConversion&lt;string&gt;()).
/// - Once a name is released, DO NOT rename or delete members; only add new members.
/// - If a rename is unavoidable, a data migration must update existing stored strings accordingly.
/// </summary>
public enum RosterCheckType
{
    /// <summary>
    /// Data quality / input validation issues (e.g. breaks exceed shift duration)
    /// </summary>
    DataQuality = 0,

    /// <summary>
    /// Minimum shift duration check (Award 10.9, 11.2-11.3)
    /// </summary>
    MinimumShiftHours = 1,

    /// <summary>
    /// Meal break requirements check (Award 16.2, Table 3)
    /// </summary>
    MealBreak = 2,

    /// <summary>
    /// Rest period between shifts check (Award 16.6)
    /// </summary>
    RestPeriodBetweenShifts = 3,

    /// <summary>
    /// Weekly hours limit check (Award 9, 10.1)
    /// </summary>
    WeeklyHoursLimit = 4,

    /// <summary>
    /// Maximum consecutive working days check (Award 15.7(e))
    /// </summary>
    MaximumConsecutiveDays = 5,
}

