using FairWorkly.Domain.Auth.Enums;

namespace FairWorkly.Application.Settings.Features.GetTeamMembers;

/// <summary>
/// DTO for team member list (GET /api/settings/team).
/// Represents a User entity in team management context.
/// </summary>
public class TeamMemberDto
{
    /// <summary>
    /// The User.Id - use this for PATCH /api/settings/team/{userId}
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Full name of the user (FirstName + LastName)
    /// </summary>
    public required string FullName { get; set; }

    /// <summary>
    /// User's email address
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// User's role (Admin or Manager). Serializes as string in JSON.
    /// </summary>
    public UserRole Role { get; set; }

    /// <summary>
    /// Whether the user account is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// When the user last logged in (null if never)
    /// </summary>
    public DateTime? LastLoginAt { get; set; }
}
