using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Auth.Enums;
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

    // Retrieve a user by the stored password reset token hash
    public async Task<User?> GetByPasswordResetTokenHashAsync(
        string passwordResetTokenHash,
        CancellationToken ct = default
    )
    {
        return await _context.Users.FirstOrDefaultAsync(
            u => u.PasswordResetToken == passwordResetTokenHash && !u.IsDeleted,
            ct
        );
    }

    // Retrieve a user by the stored invitation token hash
    public async Task<User?> GetByInvitationTokenHashAsync(
        string invitationTokenHash,
        CancellationToken ct = default
    )
    {
        return await _context.Users.FirstOrDefaultAsync(
            u => u.InvitationToken == invitationTokenHash && !u.IsDeleted,
            ct
        );
    }

    // Retrieves all users belonging to a specific organization
    public async Task<List<User>> GetByOrganizationIdAsync(
        Guid organizationId,
        CancellationToken ct = default
    )
    {
        return await _context
            .Users.Where(u => u.OrganizationId == organizationId && !u.IsDeleted)
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .ToListAsync(ct);
    }

    // Checks if the email is already taken within an organization.
    // Scoped to (OrganizationId, Email) to match the composite unique index.
    public async Task<bool> IsEmailUniqueAsync(
        Guid organizationId,
        string email,
        CancellationToken ct = default
    )
    {
        var normalized = email.Trim().ToLowerInvariant();
        return !await _context.Users.AnyAsync(
            u => u.OrganizationId == organizationId && u.Email == normalized && !u.IsDeleted,
            ct
        );
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

    // Atomically accepts a pending invitation.
    // Issues a single conditional UPDATE so that exactly one concurrent request can succeed.
    // ExecuteUpdateAsync translates directly to SQL:
    //   UPDATE users SET ... WHERE invitation_token = @hash AND invitation_status = Pending
    //                         AND invitation_token_expiry > @now AND is_deleted = false
    // The database row-level lock ensures at most one concurrent request sees rows_affected = 1.
    public async Task<int> AcceptInvitationAtomicAsync(
        string tokenHash,
        string passwordHash,
        DateTime now,
        CancellationToken ct = default
    )
    {
        var nowOffset = new DateTimeOffset(now, TimeSpan.Zero);

        return await _context
            .Users.Where(u =>
                u.InvitationToken == tokenHash
                && u.InvitationStatus == InvitationStatus.Pending
                && u.InvitationTokenExpiry != null
                && u.InvitationTokenExpiry > now
                && !u.IsDeleted
            )
            .ExecuteUpdateAsync(
                setters =>
                    setters
                        .SetProperty(u => u.PasswordHash, passwordHash)
                        .SetProperty(u => u.IsActive, true)
                        .SetProperty(u => u.InvitationStatus, InvitationStatus.Accepted)
                        .SetProperty(u => u.InvitationToken, (string?)null)
                        .SetProperty(u => u.InvitationTokenExpiry, (DateTime?)null)
                        .SetProperty(u => u.UpdatedAt, nowOffset),
                ct
            );
    }

    // Atomically resets a user's password and invalidates existing reset/refresh tokens.
    // Rotates SecurityStamp so that existing access tokens are rejected on next use.
    public async Task<int> ResetPasswordAtomicAsync(
        string tokenHash,
        string passwordHash,
        Guid newSecurityStamp,
        DateTime now,
        CancellationToken ct = default
    )
    {
        var nowOffset = new DateTimeOffset(now, TimeSpan.Zero);

        return await _context
            .Users.Where(u =>
                u.PasswordResetToken == tokenHash
                && u.PasswordResetTokenExpiry != null
                && u.PasswordResetTokenExpiry > now
                && !u.IsDeleted
            )
            .ExecuteUpdateAsync(
                setters =>
                    setters
                        .SetProperty(u => u.PasswordHash, passwordHash)
                        .SetProperty(u => u.PasswordResetToken, (string?)null)
                        .SetProperty(u => u.PasswordResetTokenExpiry, (DateTime?)null)
                        .SetProperty(u => u.RefreshToken, (string?)null)
                        .SetProperty(u => u.RefreshTokenExpiresAt, (DateTime?)null)
                        .SetProperty(u => u.SecurityStamp, newSecurityStamp)
                        .SetProperty(u => u.UpdatedAt, nowOffset),
                ct
            );
    }
}
