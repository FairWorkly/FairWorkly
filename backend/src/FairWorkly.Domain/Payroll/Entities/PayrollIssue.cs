using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Employees.Entities;

namespace FairWorkly.Domain.Payroll.Entities;

/// <summary>
/// Payroll compliance issue (underpayment, missing super, STP errors)
/// </summary>
public class PayrollIssue : BaseEntity
{
  public Guid PayrollRunId { get; set; }
  public virtual PayrollRun PayrollRun { get; set; } = null!;

  public Guid EmployeeId { get; set; }
  public virtual Employee Employee { get; set; } = null!;

  public IssueSeverity Severity { get; set; }
  public string IssueType { get; set; } = string.Empty; // "Underpayment", "Missing Super", "STP Error"
  public string Description { get; set; } = string.Empty;

  // Financial impact
  public decimal? ExpectedAmount { get; set; }
  public decimal? ActualAmount { get; set; }
  public decimal? Shortfall { get; set; }

  public string? RecommendedAction { get; set; }
}
