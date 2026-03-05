using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Application.Settings.Helpers;
using FairWorkly.Domain.Auth.Enums;
using FairWorkly.Domain.Auth.Interfaces;
using FairWorkly.Domain.Common.Result;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FairWorkly.Application.Settings.Features.ResendInvitation;

public class ResendInvitationHandler(
    IUserRepository userRepository,
    ISecretHasher secretHasher,
    IUnitOfWork unitOfWork,
    IConfiguration configuration,
    ILogger<ResendInvitationHandler> logger
) : IRequestHandler<ResendInvitationCommand, Result<ResendInvitationResponse>>
{
    public async Task<Result<ResendInvitationResponse>> Handle(
        ResendInvitationCommand request,
        CancellationToken cancellationToken
    )
    {
        // 1. Find the target user
        var user = await userRepository.GetByIdAsync(request.TargetUserId, cancellationToken);
        if (user == null)
            return Result<ResendInvitationResponse>.Of404("User not found.");

        // 2. Verify same organization — return 404 to avoid leaking cross-org membership
        if (user.OrganizationId != request.OrganizationId)
            return Result<ResendInvitationResponse>.Of404("User not found.");

        // 3. Verify user is in pending invitation status
        if (user.InvitationStatus != InvitationStatus.Pending)
        {
            return Result<ResendInvitationResponse>.Of409(
                "Can only resend invitation for users with pending status."
            );
        }

        // 4. Generate new token (invalidates old one)
        var (plainToken, tokenHash, expiresAt) = InvitationHelper.GenerateToken(
            secretHasher,
            configuration
        );

        user.SetInvitationToken(tokenHash, expiresAt);

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Build invite link
        var inviteLink = InvitationHelper.BuildInviteLink(configuration, plainToken);

        logger.LogInformation(
            "Invitation resent to {Email} (UserId: {UserId})",
            user.Email,
            user.Id
        );

        return Result<ResendInvitationResponse>.Of200(
            "Invitation resent successfully",
            new ResendInvitationResponse { InviteLink = inviteLink }
        );
    }
}
