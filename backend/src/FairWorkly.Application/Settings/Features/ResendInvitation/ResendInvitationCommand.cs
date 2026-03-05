using FairWorkly.Domain.Common.Result;
using MediatR;

namespace FairWorkly.Application.Settings.Features.ResendInvitation;

public class ResendInvitationCommand : IRequest<Result<ResendInvitationResponse>>
{
    public Guid OrganizationId { get; set; }
    public Guid TargetUserId { get; set; }
}
