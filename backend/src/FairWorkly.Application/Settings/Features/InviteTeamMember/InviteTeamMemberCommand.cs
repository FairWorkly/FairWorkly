using FairWorkly.Domain.Common.Result;
using MediatR;

namespace FairWorkly.Application.Settings.Features.InviteTeamMember;

public class InviteTeamMemberCommand : IRequest<Result<InviteTeamMemberResponse>>
{
    public Guid OrganizationId { get; set; }
    public Guid InvitedByUserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
