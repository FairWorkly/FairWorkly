using FairWorkly.Domain.Common.Result;
using MediatR;

namespace FairWorkly.Application.Settings.Features.AcceptInvitation;

public class AcceptInvitationCommand : IRequest<Result<AcceptInvitationResponse>>
{
    public string Token { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
