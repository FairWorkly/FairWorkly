using System.ComponentModel.DataAnnotations;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Enums;

namespace FairWorkly.Domain.Awards.Entities;

/// <summary>
/// Award classification level entity
/// Represents a pay classification level (1-8) within a Modern Award
/// Each level has different pay rates for Full-time, Part-time, and Casual employees
/// </summary>
public class AwardLevel : BaseEntity
{
  //  Award Reference 

  /// <summary>
  /// Which Award this level belongs to
  /// </summary>
  [Required]
  public Guid AwardId { get; set; }

  /// <summary>
  /// Navigation property to parent Award
  /// </summary>
  public virtual Award Award { get; set; } = null!;

  //  Level Information 

  /// <summary>
  /// Level number within the award (1-8)
  /// Level 1 = Introductory/Entry level
  /// Level 8 = Highest classification
  /// Example: Hospitality Award has 8 levels
  /// </summary>
  [Required]
  public int LevelNumber { get; set; }

  /// <summary>
  /// Official level name from the Award
  /// Example: "Introductory level", "Level 1", "Food and beverage attendant grade 1"
  /// Used by: Document Agent (AI prompts for contracts and position descriptions)
  /// </summary>
  [Required]
  [MaxLength(100)]
  public string LevelName { get; set; } = string.Empty;

  /// <summary>
  /// Optional description of duties/responsibilities at this level
  /// Example: "Food and beverage attendant grade 1 - an employee engaged in...
  /// serving food or beverages, clearing tables, general cleaning duties"
  /// Used by: Document Agent (AI prompts for position descriptions)
  /// </summary>
  [MaxLength(500)]
  public string? Description { get; set; }

  //  Pay Rates 
  // All rates are per hour
  // Payroll Agent uses these rates to validate payslips

  /// <summary>
  /// Hourly rate for full-time employees at this level
  /// Example: $25.41/hour (Hospitality Level 2, effective July 1, 2024)
  /// Payroll Agent checks: full-time employee pay matches this rate
  /// </summary>
  [Required]
  public decimal FullTimeHourlyRate { get; set; }

  /// <summary>
  /// Hourly rate for part-time employees at this level
  /// Usually same as full-time rate (same hourly rate, just fewer guaranteed hours)
  /// Example: $25.41/hour (same as full-time)
  /// </summary>
  [Required]
  public decimal PartTimeHourlyRate { get; set; }

  /// <summary>
  /// Hourly rate for casual employees at this level
  /// Includes 25% casual loading built in
  /// Example: $31.76/hour = $25.41 × 1.25
  /// Payroll Agent checks: casual employee pay matches this rate
  /// </summary>
  [Required]
  public decimal CasualHourlyRate { get; set; }

  //  Effective Dates 
  // Award rates change annually (usually July 1)
  // Multiple AwardLevel records exist for same Award+LevelNumber with different dates

  /// <summary>
  /// Date when these rates became effective
  /// Fair Work updates rates annually, usually July 1
  /// Example: 2024-07-01 for FY 2024/25 rates
  /// </summary>
  [Required]
  public DateTime EffectiveFrom { get; set; }

  /// <summary>
  /// Date when these rates stopped being effective
  /// Null if currently active
  /// Example: 2025-06-30 (when new rates take effect July 1, 2025)
  /// </summary>
  public DateTime? EffectiveTo { get; set; }

  /// <summary>
  /// Is this the current active rate?
  /// True for current rates, False for historical rates
  /// Query: WHERE AwardId = X AND LevelNumber = Y AND IsActive = true
  /// to get current rates
  /// </summary>
  [Required]
  public bool IsActive { get; set; } = true;
}