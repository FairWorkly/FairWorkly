using FairWorkly.Domain.Common.Result;
using MediatR;

namespace FairWorkly.Application.Settings.Features.ValidateInvitationToken;

public class ValidateInvitationTokenQuery : IRequest<Result<ValidateInvitationTokenResponse>>
{
    public string Token { get; set; } = string.Empty;
}

public class ValidateInvitationTokenResponse
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}
