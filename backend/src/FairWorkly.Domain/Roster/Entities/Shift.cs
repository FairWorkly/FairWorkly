using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Employees.Entities;

namespace FairWorkly.Domain.Roster.Entities;

/// <summary>
/// Represents a single work shift assigned to an employee
/// Part of a weekly Roster
/// Used by: Roster Agent to check shift compliance rules
/// </summary>
public class Shift : BaseEntity
{
    [Required]
    public Guid OrganizationId { get; set; }

    public virtual Organization Organization { get; set; } = null!;

    /// <summary>
    /// Which roster this shift belongs to
    /// </summary>
    [Required]
    public Guid RosterId { get; set; }

    /// <summary>
    /// Navigation property to parent roster
    /// </summary>
    public virtual Roster Roster { get; set; } = null!;

    /// <summary>
    /// Which employee is assigned to this shift
    /// </summary>
    [Required]
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// Navigation property to employee
    /// </summary>
    public virtual Employee Employee { get; set; } = null!;

    // Shift Timing

    /// <summary>
    /// Shift date (business calendar date).
    ///
    /// TIME SEMANTICS (MVP):
    /// - FairWorkly currently targets Australia only and does not support per-organization time zones yet.
    /// - Treat Date + StartTime/EndTime as local roster times in Australia/Melbourne.
    /// - Date should be stored as a date-only value (00:00:00) representing the local calendar day.
    /// - Overnight shifts are represented by EndTime &lt; StartTime (end occurs on the next calendar day).
    /// - Future: introduce Organization.TimeZoneId and interpret these fields in that time zone (incl. DST).
    /// </summary>
    [Required]
    public DateTime Date { get; set; }

    /// <summary>
    /// Shift start time
    /// Example: 09:00:00
    /// Stored as TimeSpan (time of day)
    /// </summary>
    [Required]
    public TimeSpan StartTime { get; set; }

    /// <summary>
    /// Shift end time
    /// Example: 17:00:00
    /// Stored as TimeSpan (time of day)
    /// </summary>
    [Required]
    public TimeSpan EndTime { get; set; }

    /// <summary>
    /// Shift duration in hours (computed)
    /// Handles overnight shifts (e.g. 23:00 - 02:00 = 3 hours)
    /// </summary>
    [NotMapped]
    public decimal Duration
    {
        get
        {
            var duration = EndTime - StartTime;
            if (duration.TotalHours < 0)
            {
                // Overnight shift (e.g. 11pm to 2am)
                duration = duration.Add(TimeSpan.FromDays(1));
            }
            return (decimal)duration.TotalHours;
        }
    }

    /// <summary>
    /// Shift start date and time (computed)
    /// Combines Date + StartTime for easy comparison
    /// </summary>
    [NotMapped]
    public DateTime StartDateTime => Date.Add(StartTime);

    /// <summary>
    /// Shift end date and time (computed)
    /// Handles overnight shifts
    /// </summary>
    [NotMapped]
    public DateTime EndDateTime
    {
        get
        {
            var endDateTime = Date.Add(EndTime);
            if (EndTime < StartTime)
            {
                // Overnight shift - add one day
                endDateTime = endDateTime.AddDays(1);
            }
            return endDateTime;
        }
    }

    // Break Information

    /// <summary>
    /// Does this shift have a meal break?
    /// Required if shift > MealBreakThresholdHours (from Award)
    /// </summary>
    [Required]
    public bool HasMealBreak { get; set; }

    /// <summary>
    /// Meal break duration in minutes
    /// Example: 30 minutes
    /// Null if HasMealBreak = false
    /// Roster Agent checks: duration >= Award.MealBreakMinutes
    /// </summary>
    public int? MealBreakDuration { get; set; }

    /// <summary>
    /// Does this shift have rest breaks?
    /// (short breaks, typically 10-15 minutes)
    /// </summary>
    public bool HasRestBreaks { get; set; }

    /// <summary>
    /// Total rest breaks duration in minutes
    /// Example: 20 minutes (2 × 10 minute breaks)
    /// Null if HasRestBreaks = false
    /// </summary>
    public int? RestBreaksDuration { get; set; }

    /// <summary>
    /// Net working hours (excluding breaks)
    /// Computed property
    /// </summary>
    [NotMapped]
    public decimal NetHours
    {
        get
        {
            var totalBreakMinutes = (MealBreakDuration ?? 0) + (RestBreaksDuration ?? 0);
            var net = Duration - ((decimal)totalBreakMinutes / 60);
            return net < 0 ? 0 : net;
        }
    }

    // Shift Type

    /// <summary>
    /// Reserved: Future use for public holiday rostering restrictions.
    /// Currently used by Payroll (PublicHolidayHours/Pay) but not by Roster compliance rules.
    /// </summary>
    public bool IsPublicHoliday { get; set; }

    /// <summary>
    /// Public holiday name if applicable
    /// Example: "Australia Day", "Christmas Day"
    /// </summary>
    [MaxLength(100)]
    public string? PublicHolidayName { get; set; }

    /// <summary>
    /// Reserved: Future use for on-call allowance and rostering rule calculations.
    /// </summary>
    public bool IsOnCall { get; set; }

    // Location

    /// <summary>
    /// Shift location (optional)
    /// Example: "Store #5", "Head Office", "Melbourne CBD"
    /// Used for multi-location organizations
    /// </summary>
    [MaxLength(100)]
    public string? Location { get; set; }

    // Notes

    /// <summary>
    /// Optional notes about this shift
    /// Example: "Training shift", "Cover for sick leave"
    /// </summary>
    [MaxLength(500)]
    public string? Notes { get; set; }

    // Navigation Properties

    /// <summary>
    /// Compliance issues found for this specific shift
    /// </summary>
    public virtual ICollection<RosterIssue> Issues { get; set; } = [];
}
