using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Auth.Entities;

namespace FairWorkly.Domain.Compliance.Entities;

/// <summary>
/// Roster represents a batch of uploaded shift data for compliance checking
/// </summary>
public class Roster : AuditableEntity
{
  public string FileName { get; set; } = string.Empty;
  public DateTime UploadedAt { get; set; }
  public ComplianceCheckStatus Status { get; set; }

  // Week period for this roster
  public DateTime WeekStartDate { get; set; }
  public DateTime WeekEndDate { get; set; }

  // Multi-tenancy
  public Guid OrganizationId { get; set; }
  public virtual Organization Organization { get; set; } = null!;

  // Navigation properties
  public virtual ICollection<RosterEntry> Entries { get; set; } = new List<RosterEntry>();
  public virtual ComplianceCheck? ComplianceCheck { get; set; }
}