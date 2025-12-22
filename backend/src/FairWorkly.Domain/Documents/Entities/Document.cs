using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Employees.Entities;

namespace FairWorkly.Domain.Documents.Entities;

/// <summary>
/// Document tracking entity
/// Tracks which legally required documents have been provided to employees
/// Focus: Compliance tracking, not document generation
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

  // Provision status
  /// <summary>
  /// Has this document been provided to the employee?
  /// </summary>
  [Required]
  public bool IsProvided { get; set; } = false;

  /// <summary>
  /// When was this document provided to the employee?
  /// </summary>
  public DateTimeOffset? ProvidedAt { get; set; }

  /// <summary>
  /// How was it provided?
  /// Examples: "Email", "Printed", "Downloaded", "Via HR System"
  /// </summary>
  [MaxLength(50)]
  public string? ProvidedMethod { get; set; }

  // Compliance tracking
  /// <summary>
  /// Is this document legally required by Fair Work?
  /// True: FWIS, Separation Certificate, Casual Conversion Notice
  /// False: Offer Letter (optional)
  /// </summary>
  [Required]
  public bool IsLegallyRequired { get; set; }

  /// <summary>
  /// Compliance deadline (if legally required)
  /// FWIS: Employee start date
  /// Separation Certificate: 14 days after last day
  /// Casual Conversion: 12 months after start date
  /// </summary>
  public DateTime? ComplianceDeadline { get; set; }

  /// <summary>
  /// Is compliance requirement met? (computed)
  /// True if: Not required OR provided before deadline
  /// </summary>
  [NotMapped]
  public bool IsCompliant =>
      !IsLegallyRequired ||
      !ComplianceDeadline.HasValue ||
      (ProvidedAt.HasValue && ProvidedAt.Value.DateTime <= ComplianceDeadline.Value);

  /// <summary>
  /// Days until compliance deadline (computed)
  /// Negative = overdue, Positive = days remaining
  /// </summary>
  [NotMapped]
  public int? DaysUntilDeadline =>
      ComplianceDeadline.HasValue
          ? (ComplianceDeadline.Value - DateTime.UtcNow.Date).Days
          : null;

  // Optional: User-uploaded proof
  /// <summary>
  /// Optional: User can upload a copy of the document for record keeping
  /// Example: "FWIS_AliceChen_Signed.pdf"
  /// </summary>
  [MaxLength(255)]
  public string? UploadedFileName { get; set; }

  /// <summary>
  /// Optional: File path in storage
  /// </summary>
  [MaxLength(500)]
  public string? UploadedFilePath { get; set; }

  /// <summary>
  /// Optional: File size in bytes
  /// </summary>
  public long? UploadedFileSize { get; set; }

  // Notes
  /// <summary>
  /// Optional notes from Manager
  /// Examples: "Sent via email on Jan 15", "Employee signed on receipt"
  /// </summary>
  [MaxLength(1000)]
  public string? Notes { get; set; }


}