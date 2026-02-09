using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Roster.Enums;
using FairWorkly.Domain.Roster.ValueObjects;

namespace FairWorkly.Domain.Roster.Entities;

/// <summary>
/// Represents a single roster compliance validation run
/// Created when Manager triggers roster validation
/// Used by: Roster Agent to track validation results
/// </summary>
public class RosterValidation : AuditableEntity
{
    [Required]
    public Guid OrganizationId { get; set; }
    public virtual Organization Organization { get; set; } = null!;

    /// <summary>
    /// Which roster is being validated
    /// </summary>
    [Required]
    public Guid RosterId { get; set; }

    /// <summary>
    /// Navigation property to roster
    /// </summary>
    public virtual Roster Roster { get; set; } = null!;

    // Validation Info

    /// <summary>
    /// Validation status
    /// Pending → InProgress → Passed/Failed
    /// </summary>
    [Required]
    public ValidationStatus Status { get; set; } = ValidationStatus.Pending;

    /// <summary>
    /// Week being validated
    /// </summary>
    [Required]
    public DateTime WeekStartDate { get; set; }

    [Required]
    public DateTime WeekEndDate { get; set; }

    // Validation Results

    /// <summary>
    /// Total number of shifts validated
    /// </summary>
    public int TotalShifts { get; set; }

    /// <summary>
    /// Number of shifts that passed all checks
    /// </summary>
    public int PassedShifts { get; set; }

    /// <summary>
    /// Number of shifts with issues
    /// </summary>
    public int FailedShifts { get; set; }

    /// <summary>
    /// Total number of issues found across all shifts
    /// </summary>
    public int TotalIssuesCount { get; set; }

    /// <summary>
    /// Number of failing issues (Severity >= Error) that cause validation to fail
    /// </summary>
    public int CriticalIssuesCount { get; set; }

    /// <summary>
    /// Number of unique employees affected by issues
    /// </summary>
    public int AffectedEmployees { get; set; }

    /// <summary>
    /// Pass rate percentage (computed)
    /// </summary>
    [NotMapped]
    public decimal PassRate => TotalShifts > 0 ? (decimal)PassedShifts / TotalShifts * 100 : 0;

    // ==================== Checks Performed ====================

    /// <summary>
    /// Comma-separated list of RosterCheckType values that were executed.
    /// Example: "MinimumShiftHours,MealBreak,RestPeriodBetweenShifts,WeeklyHoursLimit,MaximumConsecutiveDays"
    /// Use FormatCheckTypes() to generate, ParseCheckTypes() to read back.
    /// </summary>
    public ExecutedCheckTypeSet ExecutedCheckTypes { get; set; } = ExecutedCheckTypeSet.Empty;

    /// <summary>
    /// Formats a collection of check types into the ExecutedCheckTypes string format.
    /// </summary>
    public static string FormatCheckTypes(IEnumerable<RosterCheckType> checkTypes)
    {
        return ExecutedCheckTypeSet.FromCheckTypes(checkTypes).ToString();
    }

    /// <summary>
    /// Parses the ExecutedCheckTypes string back into a list of RosterCheckType values.
    /// Returns empty list if null or empty.
    /// </summary>
    public List<RosterCheckType> ParseCheckTypes()
    {
        return ExecutedCheckTypes.CheckTypes.ToList();
    }

    // ==================== Legacy Check Flags (Deprecated) ====================
    // These bool properties are kept for backward compatibility.
    // New code should use ExecutedCheckTypes instead.

    [Obsolete("Use ExecutedCheckTypes instead. Will be removed in future version.")]
    public bool MinimumShiftHoursCheckPerformed { get; set; } = true;

    [Obsolete("Use ExecutedCheckTypes instead. Will be removed in future version.")]
    public bool MaxConsecutiveDaysCheckPerformed { get; set; } = true;

    [Obsolete("Use ExecutedCheckTypes instead. Will be removed in future version.")]
    public bool MealBreakCheckPerformed { get; set; } = true;

    [Obsolete("Use ExecutedCheckTypes instead. Will be removed in future version.")]
    public bool RestPeriodCheckPerformed { get; set; } = true;

    [Obsolete("Use ExecutedCheckTypes instead. Will be removed in future version.")]
    public bool WeeklyHoursCheckPerformed { get; set; } = true;

    // ==================== Timing ====================

    /// <summary>
    /// When validation started
    /// Set by Service layer when validation begins
    /// </summary>
    public DateTimeOffset? StartedAt { get; set; }

    /// <summary>
    /// When validation completed
    /// Set by Service layer when validation finishes
    /// </summary>
    public DateTimeOffset? CompletedAt { get; set; }

    /// <summary>
    /// Validation duration in seconds (computed)
    /// </summary>
    [NotMapped]
    public double? DurationSeconds =>
        CompletedAt.HasValue && StartedAt.HasValue
            ? (CompletedAt.Value - StartedAt.Value).TotalSeconds
            : null;

    // ==================== Notes ====================

    /// <summary>
    /// Optional notes about this validation run
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    // ==================== Navigation Properties ====================

    /// <summary>
    /// All compliance issues found during this validation
    /// </summary>
    public virtual ICollection<RosterIssue> Issues { get; set; } = [];
}
