using FairWorkly.Domain.Auth.Enums;

namespace FairWorkly.Application.Settings.Features.UpdateTeamMember;

/// <summary>
/// Response DTO for team member update (PATCH /api/settings/team/{userId}).
/// Returns the updated values for the User.
/// </summary>
public class TeamMemberUpdatedDto
{
    /// <summary>
    /// The User.Id that was updated
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// The updated role (Admin or Manager). Serializes as string in JSON.
    /// </summary>
    public UserRole Role { get; set; }

    /// <summary>
    /// The updated active status
    /// </summary>
    public bool IsActive { get; set; }
}
