using FairWorkly.Domain.Common.Result;
using MediatR;

namespace FairWorkly.Application.Settings.Features.CancelInvitation;

public class CancelInvitationCommand : IRequest<Result<CancelInvitationResponse>>
{
    public Guid OrganizationId { get; set; }
    public Guid TargetUserId { get; set; }
}
