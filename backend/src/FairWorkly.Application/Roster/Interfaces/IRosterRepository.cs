using FairWorkly.Domain.Roster.Entities;
using RosterEntity = FairWorkly.Domain.Roster.Entities.Roster;

namespace FairWorkly.Application.Roster.Interfaces;

/// <summary>
/// Repository interface for Roster entity operations
/// </summary>
public interface IRosterRepository
{
    /// <summary>
    /// Gets a roster by ID with shifts and employees loaded
    /// </summary>
    Task<RosterEntity?> GetByIdWithShiftsAsync(
        Guid rosterId,
        Guid organizationId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Creates a new roster.
    /// Note: Does not call SaveChanges - let UnitOfWork handle transaction
    /// </summary>
    Task<RosterEntity> CreateAsync(
        RosterEntity roster,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Creates multiple shifts in bulk.
    /// Note: Does not call SaveChanges - let UnitOfWork handle transaction
    /// </summary>
    Task CreateShiftsAsync(
        IEnumerable<Shift> shifts,
        CancellationToken cancellationToken = default
    );
}
