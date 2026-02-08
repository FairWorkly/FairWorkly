using FairWorkly.Domain.Common.Enums;

namespace FairWorkly.Application.Roster.Features.ValidateRoster;

/// <summary>
/// Response from roster validation
/// </summary>
public class ValidateRosterResponse
{
    /// <summary>
    /// The validation ID
    /// </summary>
    public Guid ValidationId { get; set; }

    /// <summary>
    /// Validation status
    /// </summary>
    public ValidationStatus Status { get; set; }

    /// <summary>
    /// Total number of shifts validated
    /// </summary>
    public int TotalShifts { get; set; }

    /// <summary>
    /// Number of shifts with no issues
    /// </summary>
    public int PassedShifts { get; set; }

    /// <summary>
    /// Number of shifts with issues
    /// </summary>
    public int FailedShifts { get; set; }

    /// <summary>
    /// Total number of issues found
    /// </summary>
    public int TotalIssues { get; set; }

    /// <summary>
    /// Number of failing issues (Severity >= Error) that cause validation to fail
    /// </summary>
    public int CriticalIssues { get; set; }

    /// <summary>
    /// List of issue summaries
    /// </summary>
    public List<RosterIssueSummary> Issues { get; set; } = [];
}

/// <summary>
/// Summary of a roster issue
/// </summary>
public class RosterIssueSummary
{
    public Guid Id { get; set; }
    public Guid? ShiftId { get; set; }
    public Guid EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public string CheckType { get; set; } = string.Empty;
    public IssueSeverity Severity { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal? ExpectedValue { get; set; }
    public decimal? ActualValue { get; set; }
    public string? AffectedDates { get; set; }
}
