namespace FairWorkly.Application.Settings.Features.UpdateTeamMember;

/// <summary>
/// Request body for PATCH /api/settings/team/{id}
/// </summary>
public class UpdateTeamMemberRequest
{
    public string? Role { get; set; }
    public bool? IsActive { get; set; }
}
