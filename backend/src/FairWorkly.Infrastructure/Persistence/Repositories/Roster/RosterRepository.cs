using FairWorkly.Application.Roster.Interfaces;
using FairWorkly.Domain.Roster.Entities;
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
            .Include(r => r.RosterValidation)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<RosterEntity> CreateAsync(
        RosterEntity roster,
        CancellationToken cancellationToken = default
    )
    {
        _context.Rosters.Add(roster);
        // Note: Don't call SaveChangesAsync - let UnitOfWork handle it
        return await Task.FromResult(roster);
    }

    public async Task CreateShiftsAsync(
        IEnumerable<Shift> shifts,
        CancellationToken cancellationToken = default
    )
    {
        _context.Shifts.AddRange(shifts);
        // Note: Don't call SaveChangesAsync - let UnitOfWork handle it
        await Task.CompletedTask;
    }
}
