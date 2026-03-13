using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Domain.Auth.Interfaces;
using FairWorkly.Domain.Common.Result;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FairWorkly.Application.Auth.Features.ResetPassword;

public class ResetPasswordCommandHandler(
    IUserRepository userRepository,
    ISecretHasher secretHasher,
    IPasswordHasher passwordHasher,
    IDateTimeProvider dateTimeProvider,
    ILogger<ResetPasswordCommandHandler> logger
) : IRequestHandler<ResetPasswordCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken
    )
    {
        var tokenHash = secretHasher.Hash(request.Token);
        var now = dateTimeProvider.UtcNow.UtcDateTime;

        var user = await userRepository.GetByPasswordResetTokenHashAsync(
            tokenHash,
            cancellationToken
        );

        if (user == null)
        {
            return Result<bool>.Of404("Invalid password reset link.");
        }

        if (user.PasswordResetTokenExpiry == null || user.PasswordResetTokenExpiry <= now)
        {
            return Result<bool>.Of409(
                "This password reset link has expired. Please request a new one."
            );
        }

        var passwordHash = passwordHasher.Hash(request.Password);
        var rowsAffected = await userRepository.ResetPasswordAtomicAsync(
            tokenHash,
            passwordHash,
            Guid.NewGuid(),
            now,
            cancellationToken
        );

        if (rowsAffected == 0)
        {
            logger.LogWarning(
                "Concurrent or stale password reset rejected for UserId {UserId}",
                user.Id
            );
            return Result<bool>.Of409(
                "This password reset link is no longer valid. Please request a new one."
            );
        }

        logger.LogInformation("Password reset completed for UserId {UserId}", user.Id);

        return Result<bool>.Of200("Password reset successful.", true);
    }
}
