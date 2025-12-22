using FairWorkly.Domain.Auth.Enums;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Employees.Entities;

namespace FairWorkly.Domain.Auth.Entities;

/// <summary>
/// User entity for authentication and authorization
/// Represents people who can access the FairWorkly system
/// </summary>
public class User : AuditableEntity
{
    // Basic Information
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }

    // Role-based access control
    public UserRole Role { get; set; }

    // Multi-tenancy
    public Guid OrganizationId { get; set; }
    public virtual Organization Organization { get; set; } = null!;

    // Account status
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }

    // JWT refresh token
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiresAt { get; set; }

    // Password Reset 
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiry { get; set; }

    //for Google login
    public string? GoogleId { get; set; }

    // Employee Link (for Employee role)
    public Guid? EmployeeId { get; set; }
    public virtual Employee? Employee { get; set; }

    // Computed property
    public string FullName => $"{FirstName} {LastName}";
}
