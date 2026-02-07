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
    // Note: Emails are stored normalized (lowercase) via DbContext.SaveChangesAsync
    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == normalized, ct);
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

    // Retrieves all users belonging to a specific organization
    public async Task<List<User>> GetByOrganizationIdAsync(Guid organizationId, CancellationToken ct = default)
    {
        return await _context.Users
            .Where(u => u.OrganizationId == organizationId && !u.IsDeleted)
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .ToListAsync(ct);
    }

    // Checks if the email is already taken by another user.
    // Note: Emails are stored normalized (lowercase) via DbContext.SaveChangesAsync
    public async Task<bool> IsEmailUniqueAsync(string email, CancellationToken ct = default)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return !await _context.Users.AnyAsync(u => u.Email == normalized, ct);
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

    // Soft-deletes a user (marks as deleted, preserving audit trail).
    public void Remove(User user)
    {
        user.IsDeleted = true;
        _context.Users.Update(user);
    }
}
