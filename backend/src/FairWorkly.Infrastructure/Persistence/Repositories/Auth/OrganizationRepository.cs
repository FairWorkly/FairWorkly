using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Auth.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FairWorkly.Infrastructure.Persistence.Repositories.Auth;

public class OrganizationRepository : IOrganizationRepository
{
    private readonly FairWorklyDbContext _context;

    public OrganizationRepository(FairWorklyDbContext context)
    {
        _context = context;
    }

    // Fetches an organisation by its unique identifier.
    public async Task<Organization?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Organizations.FindAsync(new object[] { id }, ct);
    }

    // Checks if the ABN is already taken by another organization.
    public async Task<bool> IsAbnUniqueAsync(string abn, CancellationToken ct = default)
    {
        var normalized = abn.Trim();
        return !await _context.Organizations.AnyAsync(o => o.ABN == normalized && !o.IsDeleted, ct);
    }

    // Adds a new organisation to the context.
    public void Add(Organization organization)
    {
        _context.Organizations.Add(organization);
    }

    // Soft-deletes an organisation (marks as deleted, preserving audit trail).
    public void Remove(Organization organization)
    {
        organization.IsDeleted = true;
        _context.Organizations.Update(organization);
    }
}
