using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Domain.Auth.Interfaces;
using FairWorkly.Domain.Common.Result;
using MediatR;

namespace FairWorkly.Application.Auth.Features.ValidateResetPasswordToken;

public class ValidateResetPasswordTokenQueryHandler(
    IUserRepository userRepository,
    ISecretHasher secretHasher,
    IDateTimeProvider dateTimeProvider
) : IRequestHandler<ValidateResetPasswordTokenQuery, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        ValidateResetPasswordTokenQuery request,
        CancellationToken cancellationToken
    )
    {
        var tokenHash = secretHasher.Hash(request.Token);
        var user = await userRepository.GetByPasswordResetTokenHashAsync(
            tokenHash,
            cancellationToken
        );

        if (user == null)
        {
            return Result<bool>.Of404("Invalid password reset link.");
        }

        var now = dateTimeProvider.UtcNow.UtcDateTime;
        if (user.PasswordResetTokenExpiry == null || user.PasswordResetTokenExpiry <= now)
        {
            return Result<bool>.Of409(
                "This password reset link has expired. Please request a new one."
            );
        }

        return Result<bool>.Of200("Password reset token is valid.", true);
    }
}
