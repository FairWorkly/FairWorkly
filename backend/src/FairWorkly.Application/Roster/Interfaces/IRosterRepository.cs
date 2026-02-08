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
}
