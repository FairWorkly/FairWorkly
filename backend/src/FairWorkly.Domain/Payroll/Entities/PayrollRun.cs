using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Employees.Entities;
using FairWorkly.Domain.Payroll.Entities;

namespace FairWorkly.Domain.Payroll.Entities;

/// <summary>
/// Payroll run represents a single pay period check
/// NOT for payroll calculation - only for compliance validation
/// </summary>
public class PayrollRun : AuditableEntity
{
  public string PayPeriod { get; set; } = string.Empty; // e.g., "2025-W48"
  public DateTime PeriodStartDate { get; set; }
  public DateTime PeriodEndDate { get; set; }

  public PayrollCheckStatus Status { get; set; }
  public DateTime? CheckedAt { get; set; }

  // Source
  public string? SourceSystem { get; set; } // "Xero", "KeyPay", "Manual CSV"
  public string? SourceFileName { get; set; }

  // Multi-tenancy
  public Guid OrganizationId { get; set; }
  public virtual Organization Organization { get; set; } = null!;

  // Summary
  public int TotalEmployeesChecked { get; set; }
  public int SuspectedUnderpayments { get; set; }
  public decimal? TotalEstimatedShortfall { get; set; }

  // Navigation
  public virtual ICollection<PayrollEntry> Entries { get; set; } = new List<PayrollEntry>();
  public virtual ICollection<PayrollIssue> Issues { get; set; } = new List<PayrollIssue>();
}