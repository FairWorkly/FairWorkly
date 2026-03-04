using FairWorkly.Domain.Common.Result;
using MediatR;

namespace FairWorkly.Application.Settings.Features.UpdateTeamMember;

public class UpdateTeamMemberCommand : IRequest<Result<TeamMemberUpdatedDto>>
{
    public Guid CurrentUserId { get; set; } // From JWT (for self-modification checks)
    public Guid OrganizationId { get; set; } // From JWT (multi-tenancy scope)
    public Guid TargetUserId { get; set; } // User being updated (from route)
    public string? Role { get; set; }
    public bool? IsActive { get; set; }
}
