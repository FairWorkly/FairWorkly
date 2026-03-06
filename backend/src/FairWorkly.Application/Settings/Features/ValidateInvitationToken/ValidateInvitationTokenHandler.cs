using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Domain.Auth.Enums;
using FairWorkly.Domain.Auth.Interfaces;
using FairWorkly.Domain.Common.Result;
using MediatR;

namespace FairWorkly.Application.Settings.Features.ValidateInvitationToken;

public class ValidateInvitationTokenHandler(
    IUserRepository userRepository,
    ISecretHasher secretHasher
) : IRequestHandler<ValidateInvitationTokenQuery, Result<ValidateInvitationTokenResponse>>
{
    public async Task<Result<ValidateInvitationTokenResponse>> Handle(
        ValidateInvitationTokenQuery request,
        CancellationToken cancellationToken
    )
    {
        var tokenHash = secretHasher.Hash(request.Token);
        var user = await userRepository.GetByInvitationTokenHashAsync(tokenHash, cancellationToken);

        if (user == null)
        {
            return Result<ValidateInvitationTokenResponse>.Of404(
                "Invalid or expired invitation token."
            );
        }

        if (user.InvitationStatus == InvitationStatus.Accepted)
        {
            return Result<ValidateInvitationTokenResponse>.Of409(
                "This invitation has already been accepted."
            );
        }

        if (user.InvitationStatus != InvitationStatus.Pending)
        {
            return Result<ValidateInvitationTokenResponse>.Of409(
                "This invitation is no longer valid. Please ask your admin to resend the invitation."
            );
        }

        if (user.InvitationTokenExpiry == null || user.InvitationTokenExpiry <= DateTime.UtcNow)
        {
            return Result<ValidateInvitationTokenResponse>.Of409(
                "This invitation has expired. Please ask your admin to resend the invitation."
            );
        }

        return Result<ValidateInvitationTokenResponse>.Of200(
            "Invitation is valid.",
            new ValidateInvitationTokenResponse { Email = user.Email, FullName = user.FullName }
        );
    }
}
