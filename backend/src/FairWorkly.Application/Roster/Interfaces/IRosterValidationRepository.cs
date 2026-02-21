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

    /// <summary>
    /// Gets an existing (non-deleted) validation for a roster, including its issues.
    /// Returns null if no validation exists.
    /// </summary>
    Task<RosterValidation?> GetByRosterIdWithIssuesAsync(
        Guid rosterId,
        Guid organizationId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Soft deletes all non-deleted issues for a validation run.
    /// Used when retrying a previously failed execution.
    /// </summary>
    Task SoftDeleteIssuesAsync(
        Guid rosterValidationId,
        Guid organizationId,
        CancellationToken cancellationToken = default
    );
}
