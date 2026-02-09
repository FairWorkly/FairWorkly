using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Domain.Auth.Interfaces;
using FairWorkly.Domain.Common;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FairWorkly.Application.Auth.Features.ForgotPassword;

public class ForgotPasswordCommandHandler(
    IUserRepository userRepository,
    ISecretHasher secretHasher,
    IUnitOfWork unitOfWork,
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
            return Result<bool>.Success(true);
        }

        var token = secretHasher.GenerateToken(32);
        var tokenHash = secretHasher.Hash(token);

        var expiryMinutes = configuration.GetValue<int>(
            "AuthSettings:PasswordResetTokenExpiryMinutes",
            30
        );
        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        user.PasswordResetToken = tokenHash;
        user.PasswordResetTokenExpiry = expiresAt;
        userRepository.Update(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var baseUrl = configuration.GetValue<string>("Frontend:BaseUrl") ?? "http://localhost:5173";
        var normalizedBaseUrl = baseUrl.TrimEnd('/');
        var resetLink = $"{normalizedBaseUrl}/reset-password?token={token}";

        var isDevelopment = string.Equals(
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
            "Development",
            StringComparison.OrdinalIgnoreCase
        );

        // TODO: Replace with email service integration; avoid logging reset links in production.
        if (isDevelopment)
        {
            logger.LogDebug(
                "Password reset link for {Email}: {ResetLink}",
                email,
                resetLink
            );
        }
        else
        {
            logger.LogInformation("Password reset link generated for {Email}", email);
        }

        return Result<bool>.Success(true);
    }
}
