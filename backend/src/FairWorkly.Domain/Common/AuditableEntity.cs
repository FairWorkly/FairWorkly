using FairWorkly.Domain.Auth.Entities;

namespace FairWorkly.Domain.Common;

/// <summary>
/// Auditable entity with user tracking for sensitive operations
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    public Guid CreatedByUserId { get; set; }
    public virtual User? CreatedByUser { get; set; }
    public Guid? UpdatedByUserId { get; set; }
    public virtual User? UpdatedByUser { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
