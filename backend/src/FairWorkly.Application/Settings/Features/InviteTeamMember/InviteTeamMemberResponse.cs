namespace FairWorkly.Application.Settings.Features.InviteTeamMember;

public class InviteTeamMemberResponse
{
    public Guid UserId { get; set; }
    public required string InviteLink { get; set; }
}
