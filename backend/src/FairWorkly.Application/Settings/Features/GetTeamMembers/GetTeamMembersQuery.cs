using FairWorkly.Domain.Common;
using MediatR;

namespace FairWorkly.Application.Settings.Features.GetTeamMembers;

public class GetTeamMembersQuery : IRequest<Result<List<TeamMemberDto>>>
{
    public Guid CurrentUserId { get; set; }
}
