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
    ILogger<AcceptInvitationHandler> logger
) : IRequestHandler<AcceptInvitationCommand, Result<AcceptInvitationResponse>>
{
    public async Task<Result<AcceptInvitationResponse>> Handle(
        AcceptInvitationCommand request,
        CancellationToken cancellationToken
    )
    {
        var tokenHash = secretHasher.Hash(request.Token);
        var now = DateTime.UtcNow;

        // 1. Pre-read: verify the token exists and surface actionable errors early.
        //    This read is also the only source of response data (email, fullName),
        //    since ExecuteUpdateAsync does not return the affected row.
        var user = await userRepository.GetByInvitationTokenHashAsync(tokenHash, cancellationToken);

        if (user == null)
        {
            return Result<AcceptInvitationResponse>.Of404("Invalid or expired invitation token.");
        }

        if (user.InvitationStatus == InvitationStatus.Accepted)
        {
            return Result<AcceptInvitationResponse>.Of409(
                "This invitation has already been accepted."
            );
        }

        if (user.InvitationStatus != InvitationStatus.Pending)
        {
            return Result<AcceptInvitationResponse>.Of409(
                "This invitation is no longer valid. Please ask your admin to resend the invitation."
            );
        }

        if (user.InvitationTokenExpiry == null || user.InvitationTokenExpiry <= now)
        {
            return Result<AcceptInvitationResponse>.Of409(
                "This invitation has expired. Please ask your admin to resend the invitation."
            );
        }

        // 2. Atomic compare-and-set UPDATE.
        //    The WHERE clause in AcceptInvitationAtomicAsync re-checks token + Pending + expiry
        //    inside a single SQL statement, so concurrent requests racing past step 1 are
        //    serialized by the database row lock: exactly one will see rowsAffected == 1.
        var passwordHash = passwordHasher.Hash(request.Password);
        var rowsAffected = await userRepository.AcceptInvitationAtomicAsync(
            tokenHash,
            passwordHash,
            now,
            cancellationToken
        );

        if (rowsAffected == 0)
        {
            logger.LogWarning(
                "Concurrent invitation acceptance rejected for UserId {UserId}",
                user.Id
            );
            return Result<AcceptInvitationResponse>.Of409(
                "This invitation is no longer valid. It may have been accepted or cancelled."
            );
        }

        logger.LogInformation("Invitation accepted (UserId: {UserId})", user.Id);

        return Result<AcceptInvitationResponse>.Of200(
            "Invitation accepted successfully",
            new AcceptInvitationResponse { Email = user.Email, FullName = user.FullName }
        );
    }
}
