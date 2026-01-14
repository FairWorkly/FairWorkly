using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Enums;

namespace FairWorkly.Domain.Compliance.Entities;

/// <summary>
/// Represents a weekly roster (collection of shifts)
/// Created by Manager to schedule employees
/// Used by: Compliance Agent to validate roster compliance
/// </summary>
public class Roster : AuditableEntity
{
    [Required]
    public Guid OrganizationId { get; set; }
    public virtual Organization Organization { get; set; } = null!;

    //  Week Information

    /// <summary>
    /// Week start date (typically Monday)
    /// </summary>
    [Required]
    public DateTime WeekStartDate { get; set; }

    /// <summary>
    /// Week end date (typically Sunday)
    /// </summary>
    [Required]
    public DateTime WeekEndDate { get; set; }

    /// <summary>
    /// Week number in year (1-52)
    /// Example: Week 3 of 2025
    /// Used for display and filtering
    /// </summary>
    [Required]
    public int WeekNumber { get; set; }

    /// <summary>
    /// Year
    /// Example: 2025
    /// </summary>
    [Required]
    public int Year { get; set; }

    // Finalization tracking
    /// <summary>
    /// Has Manager finalized this roster?
    /// True = Roster complete, exported to employees, locked
    /// False = Still in draft, can be edited
    /// </summary>
    public bool IsFinalized { get; set; } = false;

    /// <summary>
    /// When was this roster finalized and exported?
    /// Used for audit trail
    /// </summary>
    public DateTimeOffset? FinalizedAt { get; set; }

    // Summary Statistics
    // Computed when roster is created/updated

    /// <summary>
    /// Total number of shifts in this roster
    /// </summary>
    public int TotalShifts { get; set; }

    /// <summary>
    /// Total hours across all shifts
    /// </summary>
    public decimal TotalHours { get; set; }

    /// <summary>
    /// Number of unique employees rostered
    /// </summary>
    public int TotalEmployees { get; set; }

    // Validation Reference

    /// <summary>
    /// Link to the validation that checked this roster
    /// Null if not yet validated
    /// </summary>
    public Guid? RosterValidationId { get; set; }

    /// <summary>
    /// Navigation property to validation
    /// </summary>
    public virtual RosterValidation? RosterValidation { get; set; }

    // Notes

    /// <summary>
    /// Optional notes about this roster
    /// Example: "Public holiday on Monday", "Short week due to Christmas"
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    // Navigation Properties

    /// <summary>
    /// All shifts in this roster
    /// One roster has many shifts
    /// </summary>
    public virtual ICollection<Shift> Shifts { get; set; } = new List<Shift>();

    /// <summary>
    /// All compliance issues found in this roster
    /// </summary>
    public virtual ICollection<RosterIssue> Issues { get; set; } = new List<RosterIssue>();
}
