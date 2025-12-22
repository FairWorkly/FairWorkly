using System.ComponentModel.DataAnnotations;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Enums;

namespace FairWorkly.Domain.Awards.Entities;

/// <summary>
/// Modern Award entity
/// Represents a Fair Work Modern Award with compliance rules and penalty rates
/// MVP supports 3 awards: Hospitality, Retail, Clerks
/// </summary>
public class Award : BaseEntity
{
  // Basic Information 

  /// <summary>
  /// Award type (links to AwardType enum)
  /// Used to query Award from Employee.AwardType
  /// </summary>
  [Required]
  public AwardType AwardType { get; set; }

  /// <summary>
  /// Official award name
  /// Example: "Hospitality Industry (General) Award 2020"
  /// Used by: All 3 Agents (display, AI prompts)
  /// </summary>
  [Required]
  [MaxLength(200)]
  public string Name { get; set; } = string.Empty;

  /// <summary>
  /// Fair Work award code
  /// Example: "MA000009"
  /// Used by: Document Agent (AI prompts for FWIS)
  /// </summary>
  [Required]
  [MaxLength(20)]
  public string AwardCode { get; set; } = string.Empty;

  /// <summary>
  /// Optional description
  /// </summary>
  [MaxLength(1000)]
  public string? Description { get; set; }

  // Payroll Agent Fields 
  // These fields are used by Payroll Agent to validate penalty rates

  /// <summary>
  /// Saturday penalty rate multiplier
  /// Example: 1.25 means 125% of base rate
  /// Payroll Agent checks: actualSaturdayPay == baseRate × 1.25 × hours
  /// </summary>
  [Required]
  public decimal SaturdayPenaltyRate { get; set; }

  /// <summary>
  /// Sunday penalty rate multiplier
  /// Example: 1.5 (Hospitality), 1.75 (Clerks)
  /// Different awards have different Sunday rates
  /// </summary>
  [Required]
  public decimal SundayPenaltyRate { get; set; }

  /// <summary>
  /// Public holiday penalty rate multiplier
  /// Example: 2.5 means 250% of base rate
  /// </summary>
  [Required]
  public decimal PublicHolidayPenaltyRate { get; set; }

  /// <summary>
  /// Casual loading rate
  /// Default: 0.25 (25% loading for casual employees)
  /// Payroll Agent checks: casualRate == baseRate × 1.25
  /// </summary>
  [Required]
  public decimal CasualLoadingRate { get; set; } = 0.25m;

  //  Compliance Agent Fields 
  // These fields are used by Compliance Agent to validate rosters

  /// <summary>
  /// Minimum shift duration in hours
  /// Example: 3 (cannot roster shifts shorter than 3 hours)
  /// Compliance Agent checks: shiftDuration >= 3
  /// </summary>
  [Required]
  public decimal MinimumShiftHours { get; set; }

  /// <summary>
  /// Maximum consecutive days an employee can work
  /// Example: 6 (cannot work more than 6 days in a row)
  /// Compliance Agent checks: consecutiveDays <= 6
  /// </summary>
  [Required]
  public int MaxConsecutiveDays { get; set; }

  /// <summary>
  /// Shift duration threshold requiring a meal break
  /// Example: 5 (shifts longer than 5 hours require meal break)
  /// </summary>
  [Required]
  public int MealBreakThresholdHours { get; set; }

  /// <summary>
  /// Minimum meal break duration in minutes
  /// Example: 30 (meal break must be at least 30 minutes)
  /// </summary>
  [Required]
  public int MealBreakMinutes { get; set; }

  /// <summary>
  /// Minimum rest period between shifts in hours
  /// Example: 10 (Hospitality/Clerks), 12 (Retail)
  /// Compliance Agent checks: restBetweenShifts >= 10
  /// </summary>
  [Required]
  public int MinimumRestPeriodHours { get; set; }

  /// <summary>
  /// Ordinary weekly hours for full-time employees
  /// Standard: 38 hours, overtime beyond this
  /// </summary>
  [Required]
  public int OrdinaryWeeklyHours { get; set; } = 38;

  // Navigation Properties 

  /// <summary>
  /// Award classification levels (1-8)
  /// Each level has different pay rates
  /// </summary>
  public virtual ICollection<AwardLevel> Levels { get; set; } = new List<AwardLevel>();
}
