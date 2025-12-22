using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Employees.Entities;

namespace FairWorkly.Domain.Payroll.Entities;

/// <summary>
/// Represents a single pay record for an employee
/// Created when Admin uploads payroll CSV
/// CSV template ensures all required fields are provided
/// </summary>
public class Payslip : AuditableEntity
{
  [Required]
  public Guid OrganizationId { get; set; }
  public virtual Organization Organization { get; set; } = null!;

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

  //  Hours Worked,All Required - CSV template ensures these are provided

  /// <summary>
  /// Ordinary hours (Mon-Fri, normal time)
  /// </summary>
  [Required]
  public decimal OrdinaryHours { get; set; }

  [Required]
  public decimal SaturdayHours { get; set; }

  [Required]
  public decimal SundayHours { get; set; }

  [Required]
  public decimal PublicHolidayHours { get; set; }

  public decimal OvertimeHours { get; set; } = 0;

  /// <summary>
  /// Total hours
  /// </summary>
  [NotMapped]
  public decimal TotalHours =>
      OrdinaryHours + SaturdayHours + SundayHours +
      PublicHolidayHours + OvertimeHours;

  // Gross Pay 

  /// <summary>
  /// Ordinary pay (base rate × ordinary hours)
  /// </summary>
  [Required]
  public decimal OrdinaryPay { get; set; }

  /// <summary>
  /// Saturday penalty pay
  /// </summary>
  [Required]
  public decimal SaturdayPay { get; set; }

  /// <summary>
  /// Sunday penalty pay
  /// </summary>
  [Required]
  public decimal SundayPay { get; set; }

  /// <summary>
  /// Public holiday penalty pay
  /// </summary>
  [Required]
  public decimal PublicHolidayPay { get; set; }

  /// <summary>
  /// Overtime pay
  /// </summary>
  public decimal OvertimePay { get; set; } = 0;

  /// <summary>
  /// Other allowances (meal, travel, etc.)
  /// </summary>
  public decimal Allowances { get; set; } = 0;

  /// <summary>
  /// Total gross pay before deductions
  /// </summary>
  [Required]
  public decimal GrossPay { get; set; }

  //  Deductions 

  /// <summary>
  /// Tax withheld
  /// </summary>
  [Required]
  public decimal Tax { get; set; }

  /// <summary>
  /// Superannuation contribution (employer)
  /// </summary>
  [Required]
  public decimal Superannuation { get; set; }

  /// <summary>
  /// Other deductions
  /// </summary>
  public decimal OtherDeductions { get; set; } = 0;

  /// <summary>
  /// Net pay (take-home)
  /// </summary>
  [Required]
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