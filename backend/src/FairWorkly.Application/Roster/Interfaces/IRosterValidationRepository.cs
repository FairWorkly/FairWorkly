using FairWorkly.Domain.Roster.Entities;

namespace FairWorkly.Application.Roster.Interfaces;

/// <summary>
/// Repository interface for RosterValidation entity operations
/// </summary>
public interface IRosterValidationRepository
{
    /// <summary>
    /// Creates a new roster validation record
    /// </summary>
    Task<RosterValidation> CreateAsync(
        RosterValidation validation,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Updates an existing validation record
    /// </summary>
    Task UpdateAsync(RosterValidation validation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds issues to a validation
    /// </summary>
    Task AddIssuesAsync(
        IEnumerable<RosterIssue> issues,
        CancellationToken cancellationToken = default
    );
}
