namespace FairWorkly.Application.Roster.Features.UploadRoster;

/// <summary>
/// Response model for roster upload operation.
/// Returned to frontend after successful roster file parsing and storage.
/// </summary>
public class UploadRosterResponse
{
    /// <summary>
    /// Created roster ID.
    /// Frontend navigates to /roster/results/{RosterId} upon success.
    /// </summary>
    public Guid RosterId { get; set; }

    /// <summary>
    /// Week start date from parsed roster (typically Monday).
    /// </summary>
    public DateTime WeekStartDate { get; set; }

    /// <summary>
    /// Week end date from parsed roster (typically Sunday).
    /// </summary>
    public DateTime WeekEndDate { get; set; }

    /// <summary>
    /// Total number of shifts parsed and created.
    /// </summary>
    public int TotalShifts { get; set; }

    /// <summary>
    /// Total hours across all shifts.
    /// </summary>
    public decimal TotalHours { get; set; }

    /// <summary>
    /// Number of unique employees in the roster.
    /// </summary>
    public int TotalEmployees { get; set; }

    /// <summary>
    /// Parser warnings (non-fatal issues from Agent Service).
    /// Empty list if no warnings.
    /// Frontend displays these as alerts to inform user of potential data quality issues.
    /// </summary>
    public List<ParserWarning> Warnings { get; set; } = new();
}

/// <summary>
/// Represents a non-fatal parser warning.
/// Examples: missing break duration, unusual shift length, suspected data quality issue.
/// </summary>
public class ParserWarning
{
    /// <summary>
    /// Warning code (e.g., "MEAL_BREAK_DURATION_MISSING", "EMPLOYEE_NOT_FOUND").
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable warning message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Excel row number where warning occurred (0 for roster-level warnings).
    /// </summary>
    public int Row { get; set; }

    /// <summary>
    /// Column name where warning occurred (optional).
    /// </summary>
    public string? Column { get; set; }

    /// <summary>
    /// Value that triggered the warning (optional).
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Hint or suggestion to resolve the warning (optional).
    /// </summary>
    public string? Hint { get; set; }
}
