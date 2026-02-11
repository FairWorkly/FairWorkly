using System.Text.Json.Serialization;

namespace FairWorkly.Application.Roster.Features.UploadRoster;

/// <summary>
/// Wrapper response from /api/agent/chat endpoint.
/// The endpoint wraps feature results in this structure.
/// </summary>
public class AgentChatResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("file_name")]
    public string? FileName { get; set; }

    [JsonPropertyName("routed_to")]
    public string? RoutedTo { get; set; }

    [JsonPropertyName("result")]
    public ParseResponse Result { get; set; } = null!;
}

/// <summary>
/// Response from Agent Service roster parser.
/// Maps to Python ParseResponse model from agent-service/agents/roster/services/roster_import/models.py
/// </summary>
public class ParseResponse
{
    [JsonPropertyName("result")]
    public RosterParseResult Result { get; set; } = null!;

    [JsonPropertyName("issues")]
    public List<ParseIssue> Issues { get; set; } = new();

    [JsonPropertyName("summary")]
    public ParseResultSummary Summary { get; set; } = null!;
}

/// <summary>
/// Parsed roster data with entries and computed statistics.
/// Maps to Python RosterParseResult model.
/// </summary>
public class RosterParseResult
{
    [JsonPropertyName("entries")]
    public List<RosterEntry> Entries { get; set; } = new();

    [JsonPropertyName("week_start_date")]
    public DateTime? WeekStartDate { get; set; }

    [JsonPropertyName("week_end_date")]
    public DateTime? WeekEndDate { get; set; }

    [JsonPropertyName("total_shifts")]
    public int TotalShifts { get; set; }

    [JsonPropertyName("total_hours")]
    public decimal TotalHours { get; set; }

    [JsonPropertyName("unique_employees")]
    public int UniqueEmployees { get; set; }
}

/// <summary>
/// Single roster entry (shift) parsed from Excel.
/// Maps to Python RosterEntry model.
/// </summary>
public class RosterEntry
{
    [JsonPropertyName("excel_row")]
    public int ExcelRow { get; set; }

    [JsonPropertyName("employee_email")]
    public string? EmployeeEmail { get; set; }

    [JsonPropertyName("employee_number")]
    public string? EmployeeNumber { get; set; }

    [JsonPropertyName("employee_name")]
    public string? EmployeeName { get; set; }

    [JsonPropertyName("employment_type")]
    public string? EmploymentType { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("start_time")]
    public TimeSpan StartTime { get; set; }

    [JsonPropertyName("end_time")]
    public TimeSpan EndTime { get; set; }

    [JsonPropertyName("is_overnight")]
    public bool IsOvernight { get; set; }

    [JsonPropertyName("has_meal_break")]
    public bool HasMealBreak { get; set; }

    [JsonPropertyName("meal_break_duration")]
    public int? MealBreakDuration { get; set; }

    [JsonPropertyName("has_rest_breaks")]
    public bool HasRestBreaks { get; set; }

    [JsonPropertyName("rest_breaks_duration")]
    public int? RestBreaksDuration { get; set; }

    [JsonPropertyName("is_public_holiday")]
    public bool IsPublicHoliday { get; set; }

    [JsonPropertyName("public_holiday_name")]
    public string? PublicHolidayName { get; set; }

    [JsonPropertyName("is_on_call")]
    public bool IsOnCall { get; set; }

    [JsonPropertyName("location")]
    public string? Location { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    [JsonPropertyName("duration_hours")]
    public decimal DurationHours { get; set; }

    [JsonPropertyName("net_hours")]
    public decimal NetHours { get; set; }
}

/// <summary>
/// Parser issue (error or warning).
/// Maps to Python ParseIssue model.
/// </summary>
public class ParseIssue
{
    [JsonPropertyName("severity")]
    public string Severity { get; set; } = string.Empty; // "error" or "warning"

    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("row")]
    public int Row { get; set; }

    [JsonPropertyName("column")]
    public string? Column { get; set; }

    [JsonPropertyName("value")]
    public string? Value { get; set; }

    [JsonPropertyName("hint")]
    public string? Hint { get; set; }

    [JsonPropertyName("detail")]
    public string? Detail { get; set; }
}

/// <summary>
/// Summary of parse result status and issue counts.
/// Maps to Python ParseResultSummary model.
/// </summary>
public class ParseResultSummary
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty; // "ok", "warning", "row_error", "blocking"

    [JsonPropertyName("total_issues")]
    public int TotalIssues { get; set; }

    [JsonPropertyName("error_count")]
    public int ErrorCount { get; set; }

    [JsonPropertyName("warning_count")]
    public int WarningCount { get; set; }

    [JsonPropertyName("blocking_count")]
    public int BlockingCount { get; set; }
}
