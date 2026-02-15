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

    // Checks if the email is already taken by another user.
    Task<bool> IsEmailUniqueAsync(string email, CancellationToken ct = default);

    // Registers a new user in the context.
    void Add(User user);

    // Updates an existing user's details.
    void Update(User user);

    // Removes a user from the system.
    void Remove(User user);
}
