namespace FairWorkly.Application.Settings.Features.UpdateTeamMember;

public class TeamMemberUpdatedDto
{
    public Guid Id { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
