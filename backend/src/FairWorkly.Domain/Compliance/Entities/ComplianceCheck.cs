using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Enums;

namespace FairWorkly.Domain.Compliance.Entities;

/// <summary>
/// Compliance check record for a roster
/// </summary>
public class ComplianceCheck : AuditableEntity
{
  public Guid RosterId { get; set; }
  public virtual Roster Roster { get; set; } = null!;

  public ComplianceCheckStatus Status { get; set; }
  public DateTime? CheckedAt { get; set; }

  // Summary stats
  public int TotalIssuesFound { get; set; }
  public int HighSeverityCount { get; set; }
  public int MediumSeverityCount { get; set; }
  public int LowSeverityCount { get; set; }

  // Estimated cost impact
  public decimal? EstimatedPenaltyRateCost { get; set; }
  public decimal? EstimatedOvertimeCost { get; set; }

  // AI-generated summary
  public string? AiSummary { get; set; }

  // Navigation
  public virtual ICollection<ComplianceIssue> Issues { get; set; } = new List<ComplianceIssue>();
}