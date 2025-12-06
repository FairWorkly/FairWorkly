using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Employees.Entities;

namespace FairWorkly.Domain.Compliance.Entities;

/// <summary>
/// Individual compliance issue detected
/// </summary>
public class ComplianceIssue : BaseEntity
{
  public Guid ComplianceCheckId { get; set; }
  public virtual ComplianceCheck ComplianceCheck { get; set; } = null!;

  public Guid? EmployeeId { get; set; }
  public virtual Employee? Employee { get; set; }

  public IssueSeverity Severity { get; set; }
  public string RuleViolated { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;

  // Context
  public DateTime? AffectedDate { get; set; }
  public string? AffectedShiftDetails { get; set; }

  // Suggestions
  public string? RecommendedAction { get; set; }
  public decimal? EstimatedCostImpact { get; set; }

  // Resolution tracking
  public bool IsResolved { get; set; }
  public DateTime? ResolvedAt { get; set; }
  public string? ResolutionNotes { get; set; }
}






