using FairWorkly.Domain.Auth.Enums;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Employees.Entities;
using FairWorkly.Domain.Exceptions;

namespace FairWorkly.Domain.Auth.Entities;

/// <summary>
/// User entity for authentication and authorization
/// Represents people who can access the FairWorkly system
/// </summary>
public class User : AuditableEntity, IValidatableDomain
{
    // Basic Information
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? PhoneNumber { get; set; }

    public UserRole Role { get; set; }

    // Multi-tenancy
    public Guid OrganizationId { get; set; }
    public virtual Organization Organization { get; set; } = null!;

    // Account status
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }

    // Authentication credentials (at least one must be set)
    // Password login: PasswordHash must have value
    // OAuth login: GoogleId must have value
    public string? PasswordHash { get; set; }
    public string? GoogleId { get; set; }

    // JWT refresh token
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiresAt { get; set; }

    // Password Reset
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiry { get; set; }

    // Employee Link (for Employee role)
    public Guid? EmployeeId { get; set; }
    public virtual Employee? Employee { get; set; }

    // Computed property
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// Validates domain invariants. Call before persisting.
    /// </summary>
    public void ValidateDomainRules()
    {
        if (!Enum.IsDefined(typeof(UserRole), Role))
        {
            throw new InvalidDomainStateException(
                nameof(User),
                nameof(Role),
                $"Role must be a valid value (Admin=1, Manager=2). Current value: {(int)Role}");
        }

        if (string.IsNullOrWhiteSpace(Email))
        {
            throw new InvalidDomainStateException(nameof(User), nameof(Email), "Email cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(FirstName))
        {
            throw new InvalidDomainStateException(nameof(User), nameof(FirstName), "FirstName cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(LastName))
        {
            throw new InvalidDomainStateException(nameof(User), nameof(LastName), "LastName cannot be empty");
        }

        // At least one authentication credential must be present
        var hasPassword = !string.IsNullOrWhiteSpace(PasswordHash);
        var hasGoogleId = !string.IsNullOrWhiteSpace(GoogleId);
        if (!hasPassword && !hasGoogleId)
        {
            throw new InvalidDomainStateException(
                nameof(User),
                "Credentials",
                "User must have at least one authentication credential (PasswordHash or GoogleId)");
        }
    }
}
