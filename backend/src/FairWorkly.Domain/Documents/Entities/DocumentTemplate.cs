using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Auth.Entities;

namespace FairWorkly.Domain.Documents.Entities;

/// <summary>
/// Document template for reusable contracts/letters
/// Can be system-provided or organization-custom
/// </summary>
public class DocumentTemplate : AuditableEntity
{
  public string Name { get; set; } = string.Empty;
  public DocumentType DocumentType { get; set; }
  public string TemplateHtml { get; set; } = string.Empty;

  // Template variables (JSON array of placeholder names)
  public string? Variables { get; set; } // e.g., ["employeeName", "startDate", "salary"]

  // System vs Custom
  public bool IsSystemTemplate { get; set; }

  // Multi-tenancy (null for system templates)
  public Guid? OrganizationId { get; set; }
  public virtual Organization? Organization { get; set; }

  public bool IsActive { get; set; } = true;
}

public enum DocumentStatus
{
  Draft = 1,
  PendingReview = 2,
  Approved = 3,
  Sent = 4,
  Archived = 5
}