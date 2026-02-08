using FairWorkly.Application.Roster.Interfaces;
using Microsoft.EntityFrameworkCore;
using RosterEntity = FairWorkly.Domain.Roster.Entities.Roster;

namespace FairWorkly.Infrastructure.Persistence.Repositories.Roster;

public class RosterRepository(FairWorklyDbContext context) : IRosterRepository
{
    private readonly FairWorklyDbContext _context = context;

    public async Task<RosterEntity?> GetByIdWithShiftsAsync(
        Guid rosterId,
        Guid organizationId,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .Rosters.AsNoTracking()
            .Where(r => r.Id == rosterId && r.OrganizationId == organizationId && !r.IsDeleted)
            .Include(r => r.Shifts.Where(s => !s.IsDeleted))
            .ThenInclude(s => s.Employee)
            .FirstOrDefaultAsync(cancellationToken);
    }
}

