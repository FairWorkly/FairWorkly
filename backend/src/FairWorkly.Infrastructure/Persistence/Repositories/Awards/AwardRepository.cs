using FairWorkly.Application.Awards.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FairWorkly.Infrastructure.Persistence.Repositories.Awards;

/// <summary>
/// Repository implementation for Award-related queries
/// </summary>
public class AwardRepository : IAwardRepository
{
    private readonly FairWorklyDbContext _context;

    public AwardRepository(FairWorklyDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrganizationAwardDto>> GetByOrganizationIdAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .OrganizationAwards.Where(oa => oa.OrganizationId == organizationId)
            .Join(
                _context.Awards,
                oa => oa.AwardType,
                a => a.AwardType,
                (oa, a) => new OrganizationAwardDto
                {
                    AwardType = oa.AwardType,
                    Name = a.Name,
                    AwardCode = a.AwardCode,
                    Description = a.Description,
                    IsPrimary = oa.IsPrimary,
                    EmployeeCount = oa.EmployeeCount,
                }
            )
            .OrderByDescending(x => x.IsPrimary)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }
}
