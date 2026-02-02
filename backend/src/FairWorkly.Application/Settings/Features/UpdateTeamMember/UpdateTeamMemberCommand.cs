using FairWorkly.Domain.Common;
using MediatR;

namespace FairWorkly.Application.Settings.Features.UpdateTeamMember;

public class UpdateTeamMemberCommand : IRequest<Result<TeamMemberUpdatedDto>>
{
    public Guid CurrentUserId { get; set; } // From JWT (Admin doing the update)
    public Guid TargetUserId { get; set; } // User being updated (from route)
    public string? Role { get; set; } // Optional: "Admin" or "Manager"
    public bool? IsActive { get; set; } // Optional: true or false
}
