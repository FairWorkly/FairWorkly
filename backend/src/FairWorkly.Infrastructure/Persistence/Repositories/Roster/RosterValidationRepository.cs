using FairWorkly.Application.Roster.Interfaces;
using FairWorkly.Domain.Roster.Entities;
using Microsoft.EntityFrameworkCore;

namespace FairWorkly.Infrastructure.Persistence.Repositories.Roster;

public class RosterValidationRepository(FairWorklyDbContext context) : IRosterValidationRepository
{
    private readonly FairWorklyDbContext _context = context;

    public Task<RosterValidation> CreateAsync(
        RosterValidation validation,
        CancellationToken cancellationToken = default
    )
    {
        _context.RosterValidations.Add(validation);
        return Task.FromResult(validation);
    }

    public Task UpdateAsync(RosterValidation validation, CancellationToken cancellationToken = default)
    {
        var entry = _context.Entry(validation);
        if (entry.State == EntityState.Detached)
        {
            _context.RosterValidations.Attach(validation);
            entry.State = EntityState.Modified;
        }

        return Task.CompletedTask;
    }

    public Task AddIssuesAsync(
        IEnumerable<RosterIssue> issues,
        CancellationToken cancellationToken = default
    )
    {
        _context.RosterIssues.AddRange(issues);
        return Task.CompletedTask;
    }
}

