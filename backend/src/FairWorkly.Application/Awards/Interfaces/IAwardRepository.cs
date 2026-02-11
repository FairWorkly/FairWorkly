namespace FairWorkly.Application.Awards.Interfaces;

/// <summary>
/// Repository interface for Award-related queries
/// </summary>
public interface IAwardRepository
{
    /// <summary>
    /// Gets awards configured for an organization by joining OrganizationAward with Award.
    /// </summary>
    /// <param name="organizationId">Organization ID from JWT claims</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of awards with organization-specific metadata</returns>
    Task<List<OrganizationAwardDto>> GetByOrganizationIdAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default
    );
}
