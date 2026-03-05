using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Domain.Auth.Enums;
using FairWorkly.Domain.Auth.Interfaces;
using FairWorkly.Domain.Common.Result;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FairWorkly.Application.Settings.Features.AcceptInvitation;

public class AcceptInvitationHandler(
    IUserRepository userRepository,
    ISecretHasher secretHasher,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork,
    ILogger<AcceptInvitationHandler> logger
) : IRequestHandler<AcceptInvitationCommand, Result<AcceptInvitationResponse>>
{
    public async Task<Result<AcceptInvitationResponse>> Handle(
        AcceptInvitationCommand request,
        CancellationToken cancellationToken
    )
    {
        // 1. Hash the provided token and look up user
        var tokenHash = secretHasher.Hash(request.Token);
        var user = await userRepository.GetByInvitationTokenHashAsync(tokenHash, cancellationToken);

        if (user == null)
        {
            return Result<AcceptInvitationResponse>.Of404("Invalid or expired invitation token.");
        }

        // 2. Verify invitation is still pending
        if (user.InvitationStatus != InvitationStatus.Pending)
        {
            return Result<AcceptInvitationResponse>.Of409(
                "This invitation has already been accepted."
            );
        }

        // 3. Check token expiry
        if (user.InvitationTokenExpiry == null || user.InvitationTokenExpiry < DateTime.UtcNow)
        {
            return Result<AcceptInvitationResponse>.Of409(
                "This invitation has expired. Please ask your admin to resend the invitation."
            );
        }

        // 4. Set password and activate user
        user.AcceptInvitation(passwordHasher.Hash(request.Password));
        user.ValidateDomainRules();
        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Invitation accepted by {Email} (UserId: {UserId})",
            user.Email,
            user.Id
        );

        return Result<AcceptInvitationResponse>.Of200(
            "Invitation accepted successfully",
            new AcceptInvitationResponse { Email = user.Email, FullName = user.FullName }
        );
    }
}
