using MediatR;
using FairWorkly.Domain.Common;

namespace FairWorkly.Application.Settings.Features.GetTeamMembers;

public class GetTeamMembersQuery : IRequest<Result<List<TeamMemberDto>>>
{
    public Guid CurrentUserId { get; set; }
}
