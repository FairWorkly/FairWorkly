using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Enums;

namespace FairWorkly.Domain.Compliance.Entities;

/// <summary>
/// Represents a single roster compliance validation run
/// Created when Manager triggers roster validation
/// Used by: Compliance Agent to track validation results
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
  /// Number of critical issues
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
  public decimal PassRate =>
      TotalShifts > 0
          ? (decimal)PassedShifts / TotalShifts * 100
          : 0;

  // ==================== Checks Performed ====================
  // Track which of the 4-5 compliance checks were performed

  /// <summary>
  /// Was minimum shift hours check performed?
  /// Checks: shift.Duration >= Award.MinimumShiftHours
  /// </summary>
  public bool MinimumShiftHoursCheckPerformed { get; set; } = true;

  /// <summary>
  /// Was maximum consecutive days check performed?
  /// Checks: employee consecutive work days <= Award.MaxConsecutiveDays
  /// </summary>
  public bool MaxConsecutiveDaysCheckPerformed { get; set; } = true;

  /// <summary>
  /// Was meal break check performed?
  /// Checks: shifts > threshold have adequate meal breaks
  /// </summary>
  public bool MealBreakCheckPerformed { get; set; } = true;

  /// <summary>
  /// Was rest period check performed?
  /// Checks: rest between shifts >= Award.MinimumRestPeriodHours
  /// </summary>
  public bool RestPeriodCheckPerformed { get; set; } = true;

  /// <summary>
  /// Was weekly hours limit check performed?
  /// Checks: full-time employees don't exceed ordinary hours + reasonable overtime
  /// </summary>
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
  public virtual ICollection<RosterIssue> Issues { get; set; } = new List<RosterIssue>();
}