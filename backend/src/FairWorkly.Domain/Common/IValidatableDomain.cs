namespace FairWorkly.Domain.Common;

/// <summary>
/// Interface for domain entities that require validation before persistence.
/// Implement this interface on entities with complex business rules that should
/// be enforced both in the domain layer and as a fallback in the infrastructure layer.
/// </summary>
public interface IValidatableDomain
{
    /// <summary>
    /// Validates domain invariants. Throws <see cref="Exceptions.InvalidDomainStateException"/>
    /// if the entity is in an invalid state.
    /// </summary>
    /// <remarks>
    /// This method is called:
    /// 1. Explicitly in Application layer command handlers (for friendly error messages)
    /// 2. Automatically by DbContext.SaveChangesAsync as a safety net
    /// </remarks>
    void ValidateDomainRules();
}
