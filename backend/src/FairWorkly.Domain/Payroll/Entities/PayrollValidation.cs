using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Enums;

namespace FairWorkly.Domain.Payroll.Entities;

/// <summary>
/// Represents a single payroll validation run
/// Created when Admin uploads CSV and triggers validation
/// </summary>
public class PayrollValidation : AuditableEntity
{
  [Required]
  public Guid OrganizationId { get; set; }
  public virtual Organization Organization { get; set; } = null!;

  // Validation Info 

  /// <summary>
  /// Validation status
  /// Starts as Pending, becomes InProgress, then Passed/Failed
  /// </summary>
  [Required]
  public ValidationStatus Status { get; set; } = ValidationStatus.Pending;

  /// <summary>
  /// Pay period being validated
  /// </summary>
  [Required]
  public DateTime PayPeriodStart { get; set; }

  [Required]
  public DateTime PayPeriodEnd { get; set; }

  // Upload Info

  /// <summary>
  /// Uploaded CSV file path(server storage location)
  /// </summary>
  [MaxLength(500)]
  public string? FilePath { get; set; }

  /// <summary>
  /// Original filename
  /// </summary>
  [MaxLength(255)]
  public string? FileName { get; set; }

  /// <summary>
  /// Number of payslips in this validation
  /// </summary>
  public int TotalPayslips { get; set; }

  //Validation Results

  /// <summary>
  /// Number of payslips that passed all checks
  /// </summary>
  public int PassedCount { get; set; }

  /// <summary>
  /// Number of payslips with issues
  /// </summary>
  public int FailedCount { get; set; }

  /// <summary>
  /// Total number of issues found
  /// </summary>
  public int TotalIssuesCount { get; set; }

  /// <summary>
  /// Number of critical issues
  /// </summary>
  public int CriticalIssuesCount { get; set; }

  // Checks Performed,Track which of the 5 checks were performed

  /// <summary>
  /// Was base rate check performed?
  /// </summary>
  public bool BaseRateCheckPerformed { get; set; } = true;

  /// <summary>
  /// Was penalty rate check performed?
  /// </summary>
  public bool PenaltyRateCheckPerformed { get; set; } = true;

  /// <summary>
  /// Was casual loading check performed?
  /// </summary>
  public bool CasualLoadingCheckPerformed { get; set; } = true;

  /// <summary>
  /// Was superannuation check performed?
  /// </summary>
  public bool SuperannuationCheckPerformed { get; set; } = true;

  /// <summary>
  /// Was STP compliance check performed?
  /// </summary>
  public bool STPCheckPerformed { get; set; } = true;

  // Timing 

  /// <summary>
  /// When validation started
  /// </summary>
  public DateTimeOffset? StartedAt { get; set; }

  /// <summary>
  /// When validation completed
  /// </summary>
  public DateTimeOffset? CompletedAt { get; set; }

  /// <summary>
  /// Validation duration in seconds
  /// </summary>
  [NotMapped]
  public double? DurationSeconds =>
      CompletedAt.HasValue && StartedAt.HasValue
          ? (CompletedAt.Value - StartedAt.Value).TotalSeconds
          : null;

  // Optional notes about this validation run

  [MaxLength(1000)]
  public string? Notes { get; set; }

  //  Navigation Properties

  public virtual ICollection<Payslip> Payslips { get; set; } = new List<Payslip>();
  public virtual ICollection<PayrollIssue> Issues { get; set; } = new List<PayrollIssue>();
}