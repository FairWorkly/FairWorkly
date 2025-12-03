namespace FairWorkly.Domain.Common;

/// <summary>
/// Base entity with audit fields for all domain entities
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } // Soft delete support
}
