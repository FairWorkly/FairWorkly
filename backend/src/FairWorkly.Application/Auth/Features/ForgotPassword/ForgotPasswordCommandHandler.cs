using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Domain.Auth.Interfaces;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Result;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FairWorkly.Application.Auth.Features.ForgotPassword;

public class ForgotPasswordCommandHandler(
    IUserRepository userRepository,
    ISecretHasher secretHasher,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider,
    IConfiguration configuration,
    ILogger<ForgotPasswordCommandHandler> logger
) : IRequestHandler<ForgotPasswordCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken
    )
    {
        var email = request.Email.Trim();
        var user = await userRepository.GetByEmailAsync(email, cancellationToken);

        // Always return success to avoid leaking which emails exist.
        if (user == null)
        {
            logger.LogInformation("Forgot password requested for unknown email: {Email}", email);
            return Result<bool>.Of200("Password reset requested", true);
        }

        var token = secretHasher.GenerateToken(32);
        var tokenHash = secretHasher.Hash(token);

        var expiryMinutes = configuration.GetValue<int>(
            "AuthSettings:PasswordResetTokenExpiryMinutes",
            30
        );
        var expiresAt = dateTimeProvider.UtcNow.UtcDateTime.AddMinutes(expiryMinutes);

        user.PasswordResetToken = tokenHash;
        user.PasswordResetTokenExpiry = expiresAt;
        userRepository.Update(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var baseUrl = configuration.GetValue<string>("Frontend:BaseUrl") ?? "http://localhost:5173";
        var normalizedBaseUrl = baseUrl.TrimEnd('/');
        var resetLink = $"{normalizedBaseUrl}/reset-password?token={token}";
        var environmentName =
            configuration["ASPNETCORE_ENVIRONMENT"]
            ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        // TODO: Replace with email service integration; avoid logging reset links in production.
        if (string.Equals(environmentName, "Development", StringComparison.OrdinalIgnoreCase))
        {
            logger.LogInformation("Password reset link for {Email}: {ResetLink}", email, resetLink);
        }
        else
        {
            logger.LogInformation("Password reset link generated for {Email}", email);
        }

        return Result<bool>.Of200("Password reset requested", true);
    }
}
