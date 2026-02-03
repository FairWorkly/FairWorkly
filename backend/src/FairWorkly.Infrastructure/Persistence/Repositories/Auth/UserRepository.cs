using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Auth.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FairWorkly.Infrastructure.Persistence.Repositories.Auth;

public class UserRepository : IUserRepository
{
    private readonly FairWorklyDbContext _context;

    public UserRepository(FairWorklyDbContext context)
    {
        _context = context;
    }

    // Fetches a user by their unique identifier.
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Users.FindAsync(new object[] { id }, ct);
    }

    // Retrieves a user record by their email address.
    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
    }

    // Retrieve a user by the stored refresh token hash
    public async Task<User?> GetByRefreshTokenHashAsync(
        string refreshTokenHash,
        CancellationToken ct = default
    )
    {
        return await _context.Users.FirstOrDefaultAsync(
            u => u.RefreshToken == refreshTokenHash,
            ct
        );
    }

    // Checks if the email is already taken by another user.
    public async Task<bool> IsEmailUniqueAsync(string email, CancellationToken ct = default)
    {
        // Returns true if no matching email is found.
        return !await _context.Users.AnyAsync(u => u.Email == email, ct);
    }

    // Registers a new user in the context.
    public void Add(User user)
    {
        _context.Users.Add(user);
    }

    // Updates an existing user's details.
    public void Update(User user)
    {
        _context.Users.Update(user);
    }

    // Removes a user from the system.
    public void Remove(User user)
    {
        _context.Users.Remove(user);
    }
}
