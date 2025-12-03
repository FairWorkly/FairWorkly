using FairWorkly.Application.Common.Interfaces;

namespace FairWorkly.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly FairWorklyDbContext _context;

    public UnitOfWork(FairWorklyDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
