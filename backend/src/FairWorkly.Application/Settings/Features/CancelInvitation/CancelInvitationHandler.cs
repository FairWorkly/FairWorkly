using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Domain.Auth.Enums;
using FairWorkly.Domain.Auth.Interfaces;
using FairWorkly.Domain.Common.Result;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FairWorkly.Application.Settings.Features.CancelInvitation;

public class CancelInvitationHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ILogger<CancelInvitationHandler> logger
) : IRequestHandler<CancelInvitationCommand, Result<CancelInvitationResponse>>
{
    public async Task<Result<CancelInvitationResponse>> Handle(
        CancelInvitationCommand request,
        CancellationToken cancellationToken
    )
    {
        // 1. Find the target user
        var user = await userRepository.GetByIdAsync(request.TargetUserId, cancellationToken);
        if (user == null)
            return Result<CancelInvitationResponse>.Of404("User not found.");

        // 2. Verify same organization — return 404 to avoid leaking cross-org membership
        if (user.OrganizationId != request.OrganizationId)
            return Result<CancelInvitationResponse>.Of404("User not found.");

        // 3. Verify user is in pending invitation status
        if (user.InvitationStatus != InvitationStatus.Pending)
        {
            return Result<CancelInvitationResponse>.Of409(
                "Can only cancel invitations for users with pending status."
            );
        }

        // 4. Soft-delete the pending user
        user.CancelInvitation();

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Invitation cancelled for {Email} (UserId: {UserId})",
            user.Email,
            user.Id
        );

        return Result<CancelInvitationResponse>.Of200(
            "Invitation cancelled successfully",
            new CancelInvitationResponse { UserId = user.Id }
        );
    }
}
