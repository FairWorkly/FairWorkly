using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Employees.Entities;

namespace FairWorkly.Domain.Documents.Entities;

/// <summary>
/// Document entity for contracts, letters, and templates
/// </summary>
public class Document : AuditableEntity
{
  public string Title { get; set; } = string.Empty;
  public DocumentType DocumentType { get; set; }
  public DocumentStatus Status { get; set; }

  // Content
  public string? ContentHtml { get; set; } // Draft HTML content(optional)
  public string? ContentPdf { get; set; }  // Final PDF file path/S3

  // Template used (if applicable)
  public Guid? TemplateId { get; set; }
  public virtual DocumentTemplate? Template { get; set; }

  // Related entities
  public Guid? EmployeeId { get; set; }
  public virtual Employee? Employee { get; set; }

  // Multi-tenancy
  public Guid OrganizationId { get; set; }
  public virtual Organization Organization { get; set; } = null!;

  // Generation metadata
  public DateTime? GeneratedAt { get; set; }
  public string? GeneratedBy { get; set; }
  public bool IsAiGenerated { get; set; }

  // Compliance notes
  public string? ComplianceNotes { get; set; }
  public bool HasComplianceWarnings { get; set; }
}