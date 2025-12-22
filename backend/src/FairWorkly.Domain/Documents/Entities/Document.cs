using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Employees.Entities;

namespace FairWorkly.Domain.Documents.Entities;

/// <summary>
/// Represents AI-generated compliance and employment documents
/// Primary use: Legal compliance (FWIS, Separation Certificates)
/// Secondary use: Professional documents (Offer Letters)
/// </summary>
public class Document : AuditableEntity
{

  [Required]
  public Guid OrganizationId { get; set; }
  public virtual Organization Organization { get; set; } = null!;

  /// <summary>
  /// Which employee this document is for (null for org-level documents)
  /// </summary>
  public Guid? EmployeeId { get; set; }
  public virtual Employee? Employee { get; set; }

  // Document type & metadata
  [Required]
  public DocumentType DocumentType { get; set; }

  /// <summary>
  /// Document title
  /// Example: "Fair Work Information Statement - Alice Chen"
  /// </summary>
  [Required]
  [MaxLength(200)]
  public string Title { get; set; } = string.Empty;

  /// <summary>
  /// Document description (optional)
  /// Example: "FWIS for new Bartender starting Jan 20, 2025"
  /// </summary>
  [MaxLength(500)]
  public string? Description { get; set; }

  // File storage
  /// <summary>
  /// File name for download
  /// Example: "FWIS_AliceChen_20250120.pdf"
  /// </summary>
  [Required]
  [MaxLength(255)]
  public string FileName { get; set; } = string.Empty;

  /// <summary>
  /// File path in storage (S3)
  /// Example: "/documents/org_abc123/fwis/2025/FWIS_AliceChen_20250120.pdf"
  /// </summary>
  [Required]
  [MaxLength(500)]
  public string FilePath { get; set; } = string.Empty;

  /// <summary>
  /// File size in bytes
  /// </summary>
  public long FileSize { get; set; }

  /// <summary>
  /// MIME type
  /// Typically "application/pdf" for PDF documents
  /// "application/vnd.openxmlformats-officedocument.wordprocessingml.document" for DOCX
  /// </summary>
  [Required]
  [MaxLength(100)]
  public string MimeType { get; set; } = "application/pdf";

  // AI generation details
  /// <summary>
  /// AI-generated content (Markdown/HTML before PDF conversion)
  /// Stored for potential regeneration or editing
  /// </summary>
  public string? GeneratedContent { get; set; }

  /// <summary>
  /// AI prompt used to generate this document
  /// Stored for audit trail and debugging
  /// Example: "Generate FWIS for {EmployeeName}, Award: {AwardName}, Start Date: {Date}"
  /// </summary>
  public string? GenerationPrompt { get; set; }

  /// <summary>
  /// AI model used for generation
  /// Example: "claude-sonnet-4-20250514"
  /// </summary>
  [MaxLength(100)]
  public string? AIModel { get; set; }

  /// <summary>
  /// Token usage for cost tracking
  /// Helps monitor API costs
  /// </summary>
  public int? TokensUsed { get; set; }

  /// <summary>
  /// Generation time in milliseconds
  /// For performance monitoring
  /// </summary>
  public int? GenerationTimeMs { get; set; }

  // Document status
  /// <summary>
  /// Document status workflow
  /// Draft = AI generated, not yet finalized
  /// Final = Approved, ready to send to employee
  /// Sent = Delivered to employee
  /// Archived = Superseded by newer version
  /// </summary>
  [Required]
  [MaxLength(20)]
  public string Status { get; set; } = "Draft";

  /// <summary>
  /// When was this document finalized?
  /// Manager clicks "Approve" → Status becomes "Final"
  /// </summary>
  public DateTimeOffset? FinalizedAt { get; set; }

  /// <summary>
  /// When was this document sent to employee?
  /// </summary>
  public DateTimeOffset? SentAt { get; set; }

  /// <summary>
  /// How was it delivered?
  /// "Email", "Download", "Printed", "Portal"
  /// </summary>
  [MaxLength(50)]
  public string? DeliveryMethod { get; set; }

  // Legal compliance tracking
  /// <summary>
  /// Is this document legally required by Fair Work?
  /// True for: FWIS, Separation Certificate, Casual Conversion Notice
  /// False for: Offer Letter, Employment Contract, Position Description
  /// </summary>
  public bool IsLegallyRequired { get; set; }

  /// <summary>
  /// Compliance deadline (if legally required)
  /// FWIS: Before first day of work
  /// Separation Certificate: Within 14 days of termination
  /// Casual Conversion: When employee becomes eligible (12 months)
  /// </summary>
  public DateTime? ComplianceDeadline { get; set; }

  /// <summary>
  /// Is compliance requirement met? (computed)
  /// True if: Not required OR Sent before deadline
  /// </summary>
  [NotMapped]
  public bool IsCompliant =>
      !IsLegallyRequired ||
      !ComplianceDeadline.HasValue ||
      (SentAt.HasValue && SentAt.Value.DateTime <= ComplianceDeadline.Value);

  /// <summary>
  /// Days until compliance deadline (computed)
  /// Negative = overdue
  /// </summary>
  [NotMapped]
  public int? DaysUntilDeadline =>
      ComplianceDeadline.HasValue
          ? (ComplianceDeadline.Value - DateTime.UtcNow.Date).Days
          : null;

  // Version control
  /// <summary>
  /// Document version number
  /// Starts at 1, increments when regenerated
  /// Example: Manager regenerates FWIS with updated info → Version 2
  /// </summary>
  public int Version { get; set; } = 1;

  /// <summary>
  /// Reference to previous version (if this is a regeneration)
  /// Null for first version
  /// </summary>
  public Guid? PreviousVersionId { get; set; }

  /// <summary>
  /// Navigation to previous version
  /// </summary>
  public virtual Document? PreviousVersion { get; set; }

  // Additional metadata
  /// <summary>
  /// Optional notes from Manager
  /// Example: "Regenerated with updated salary", "Sent via email on Jan 15"
  /// </summary>
  [MaxLength(1000)]
  public string? Notes { get; set; }

  /// <summary>
  /// Tags for categorization/search
  /// Example: "urgent", "new-hire", "termination"
  /// Comma-separated
  /// </summary>
  [MaxLength(200)]
  public string? Tags { get; set; }
}