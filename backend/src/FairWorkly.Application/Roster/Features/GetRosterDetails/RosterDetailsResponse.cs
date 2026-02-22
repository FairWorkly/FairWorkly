namespace FairWorkly.Application.Roster.Features.GetRosterDetails;

/// <summary>
/// Response model for roster details.
/// Returned to frontend to display roster summary and shift breakdown.
/// </summary>
public class RosterDetailsResponse
{
    public Guid RosterId { get; set; }
    public DateTime WeekStartDate { get; set; }
    public DateTime WeekEndDate { get; set; }
    public int WeekNumber { get; set; }
    public int Year { get; set; }
    public int TotalShifts { get; set; }
    public decimal TotalHours { get; set; }
    public int TotalEmployees { get; set; }
    public bool IsFinalized { get; set; }
    public string? OriginalFileName { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Whether compliance validation has been run for this roster.
    /// </summary>
    public bool HasValidation { get; set; }

    /// <summary>
    /// Validation ID if validation has been run.
    /// </summary>
    public Guid? ValidationId { get; set; }

    /// <summary>
    /// Shifts grouped by employee for display.
    /// </summary>
    public List<EmployeeShiftGroup> Employees { get; set; } = [];
}

public class EmployeeShiftGroup
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string? EmployeeNumber { get; set; }
    public int ShiftCount { get; set; }
    public decimal TotalHours { get; set; }
    public List<ShiftSummary> Shifts { get; set; } = [];
}

public class ShiftSummary
{
    public Guid ShiftId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public decimal Duration { get; set; }
    public bool HasMealBreak { get; set; }
    public int? MealBreakDuration { get; set; }
    public string? Location { get; set; }
}
