namespace FairWorkly.Application.Settings.Features.GetTeamMembers;

/// <summary>
/// DTO for team member data returned to frontend.
/// Maps from User entity, filtering sensitive fields.
/// </summary>
public class TeamMemberDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // "Admin" or "Manager"
    public bool IsActive { get; set; }
    public DateTimeOffset? LastLoginAt { get; set; }
}
