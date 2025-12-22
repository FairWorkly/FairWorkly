using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Employees.Entities;

namespace FairWorkly.Domain.Payroll.Entities;

/// <summary>
/// Payslip entity - Historical payroll record (snapshot at time of pay period)
/// 
/// DESIGN PRINCIPLE:
/// - All employees (Full-time/Part-time/Casual) use OrdinaryHours/OrdinaryPay for regular work
/// - Casual employees have additional CasualLoadingPay field (25% loading on all hours)
/// - No need to separate hours by employment type
/// </summary>
public class Payslip : AuditableEntity
{
  [Required]
  public Guid OrganizationId { get; set; }
  public virtual Organization Organization { get; set; } = null!;

  /// <summary>
  /// Reference to employee
  /// WARNING: Employee record may have changed since this payslip was created
  /// Use snapshot fields (EmploymentType, AwardType, etc.) for accurate historical data
  /// </summary>

  [Required]
  public Guid EmployeeId { get; set; }
  public virtual Employee Employee { get; set; } = null!;

  /// <summary>
  /// Link to the validation that checked this payslip
  /// </summary>
  public Guid? PayrollValidationId { get; set; }
  public virtual PayrollValidation? PayrollValidation { get; set; }

  // Pay Period 

  [Required]
  public DateTime PayPeriodStart { get; set; }

  [Required]
  public DateTime PayPeriodEnd { get; set; }

  [Required]
  public DateTime PayDate { get; set; }

  // Snapshot: Employee status at time of this pay period
  /// <summary>
  /// Employment type at time of this pay period
  /// Snapshot - do NOT use Employee.EmploymentType for historical payslips
  /// </summary>
  [Required]
  public EmploymentType EmploymentType { get; set; }

  /// <summary>
  /// Award type at time of this pay period
  /// Snapshot - do NOT use Employee.AwardType for historical payslips
  /// </summary>
  [Required]
  public AwardType AwardType { get; set; }

  /// <summary>
  /// Award classification level at time of this pay period
  /// Example: "Level 2", "Grade 3", "Year 1"
  /// Snapshot - do NOT use Employee.AwardLevelNumber for historical payslips
  /// </summary>
  [Required]
  [MaxLength(50)]
  public string Classification { get; set; } = string.Empty;

  /// <summary>
  /// Base hourly rate at time of this pay period (before penalties/loading)
  /// Snapshot - Award rates may have changed since then
  /// </summary>
  public decimal HourlyRate { get; set; }

  //  Hours Worked,All Required - CSV template ensures these are provided

  /// <summary>
  /// Ordinary hours worked (Monday-Friday standard hours)
  /// Used by ALL employee types (Full-time/Part-time/Casual)
  /// </summary>
  public decimal OrdinaryHours { get; set; }

  public decimal SaturdayHours { get; set; }

  public decimal SundayHours { get; set; }

  public decimal PublicHolidayHours { get; set; }

  public decimal? OvertimeHours { get; set; }


  /// <summary>
  /// Ordinary pay (base rate only, no loading)
  /// Calculation: OrdinaryHours × HourlyRate
  /// </summary>
  public decimal OrdinaryPay { get; set; }


  /// <summary>
  /// Saturday penalty pay
  /// </summary>
  public decimal SaturdayPay { get; set; }

  /// <summary>
  /// Sunday penalty pay
  /// </summary>
  public decimal SundayPay { get; set; }

  /// <summary>
  /// Public holiday penalty pay
  /// </summary>
  public decimal PublicHolidayPay { get; set; }

  /// <summary>
  /// Overtime pay
  /// </summary>
  public decimal? OvertimePay { get; set; }

  /// <summary>
  /// Other allowances (meal, travel, etc.)
  /// </summary>
  public decimal? Allowances { get; set; }

  /// <summary>
  /// Casual loading pay (25% extra for casual employees)
  /// Null for Full-time/Part-time employees
  /// </summary>
  public decimal? CasualLoadingPay { get; set; }

  /// <summary>
  /// Total gross pay before deductions
  /// </summary
  public decimal GrossPay { get; set; }

  //  Deductions 

  /// <summary>
  /// Tax withheld
  /// </summary>
  public decimal Tax { get; set; }

  /// <summary>
  /// Superannuation contribution (employer)
  /// </summary>
  public decimal Superannuation { get; set; }

  /// <summary>
  /// Other deductions
  /// </summary>
  public decimal? OtherDeductions { get; set; }

  /// <summary>
  /// Net pay (take-home)
  /// </summary>
  public decimal NetPay { get; set; }

  // Source Data 

  /// <summary>
  /// Original CSV row data (for audit trail and debugging)
  /// </summary>
  [MaxLength(2000)]
  public string? SourceData { get; set; }

  /// <summary>
  /// External payroll system reference (Xero/MYOB ID)(optional)
  /// </summary>
  [MaxLength(100)]
  public string? ExternalReference { get; set; }

  // Navigation Properties 

  public virtual ICollection<PayrollIssue> Issues { get; set; } = new List<PayrollIssue>();
}