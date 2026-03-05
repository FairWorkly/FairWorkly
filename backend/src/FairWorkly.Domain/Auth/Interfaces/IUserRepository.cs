using FairWorkly.Domain.Auth.Entities;

namespace FairWorkly.Domain.Auth.Interfaces;

public interface IUserRepository
{
    // Fetches a user by their unique identifier.
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);

    // Retrieves a user record by their email address.
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);

    // Retrieve a user by the stored refresh token hash
    Task<User?> GetByRefreshTokenHashAsync(string refreshTokenHash, CancellationToken ct = default);

    // Retrieves all users belonging to a specific organization
    Task<List<User>> GetByOrganizationIdAsync(Guid organizationId, CancellationToken ct = default);

    // Retrieve a user by the stored invitation token hash
    Task<User?> GetByInvitationTokenHashAsync(
        string invitationTokenHash,
        CancellationToken ct = default
    );

    // Atomically accepts a pending invitation via a single conditional UPDATE.
    // Returns 1 if the row was updated, 0 if the token no longer matches a Pending,
    // non-expired user (concurrent accept, cancellation, or expiry between pre-read and update).
    Task<int> AcceptInvitationAtomicAsync(
        string tokenHash,
        string passwordHash,
        DateTime now,
        CancellationToken ct = default
    );

    // Checks if the email is already taken within an organization (matches composite unique index).
    Task<bool> IsEmailUniqueAsync(
        Guid organizationId,
        string email,
        CancellationToken ct = default
    );

    // Registers a new user in the context.
    void Add(User user);

    // Updates an existing user's details.
    void Update(User user);

    // Removes a user from the system.
    void Remove(User user);
}
