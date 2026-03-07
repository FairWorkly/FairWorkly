using FairWorkly.Domain.Auth.Entities;

namespace FairWorkly.Domain.Auth.Interfaces;

public interface IOrganizationRepository
{
    // Fetches an organisation by its unique identifier.
    Task<Organization?> GetByIdAsync(Guid id, CancellationToken ct = default);

    // Fetches an organisation with its OrganizationAwards collection included.
    Task<Organization?> GetByIdWithAwardsAsync(Guid id, CancellationToken ct = default);

    // Checks if the ABN is already taken by another organization.
    Task<bool> IsAbnUniqueAsync(string abn, CancellationToken ct = default);

    // Adds a new organisation to the context.
    void Add(Organization organization);

    // Removes an organisation from the system.
    void Remove(Organization organization);
}
