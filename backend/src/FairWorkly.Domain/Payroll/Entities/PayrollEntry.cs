using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Employees.Entities;


namespace FairWorkly.Domain.Payroll.Entities;

/// <summary>
/// Individual employee's pay data for a pay run
/// </summary>
public class PayrollEntry : BaseEntity
{
  public Guid PayrollRunId { get; set; }
  public virtual PayrollRun PayrollRun { get; set; } = null!;

  public Guid EmployeeId { get; set; }
  public virtual Employee Employee { get; set; } = null!;

  // Pay components
  public decimal OrdinaryHours { get; set; }
  public decimal OrdinaryPay { get; set; }
  public decimal OvertimeHours { get; set; }
  public decimal OvertimePay { get; set; }
  public decimal PenaltyHours { get; set; }
  public decimal PenaltyPay { get; set; }
  public decimal Allowances { get; set; }

  // Super
  public decimal SuperContribution { get; set; }

  // Deductions
  public decimal TotalDeductions { get; set; }

  // Net
  public decimal GrossPay { get; set; }
  public decimal NetPay { get; set; }

  // STP Phase 2 fields
  public string? EmploymentBasis { get; set; } // "F", "P", "C"
  public string? IncomeType { get; set; }
}