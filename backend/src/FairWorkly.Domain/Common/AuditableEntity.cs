namespace FairWorkly.Domain.Common;

/// <summary>
/// Auditable entity with user tracking for sensitive operations
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
}
