namespace FairWorkly.Application.Settings.Features.InviteTeamMember;

public class InviteTeamMemberRequest
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
