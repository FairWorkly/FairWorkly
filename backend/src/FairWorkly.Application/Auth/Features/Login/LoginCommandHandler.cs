using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Domain.Auth.Interfaces;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Result;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace FairWorkly.Application.Auth.Features.Login;

public class LoginCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    ISecretHasher secretHasher,
    IUnitOfWork unitOfWork,
    IConfiguration configuration
) : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken
    )
    {
        var email = request.Email.Trim();
        var user = await userRepository.GetByEmailAsync(email, cancellationToken);

        // User must exist AND have a password hash (OAuth-only users cannot use password login)
        if (
            user == null
            || string.IsNullOrWhiteSpace(user.PasswordHash)
            || !passwordHasher.Verify(request.Password, user.PasswordHash)
        )
        {
            return Result<LoginResponse>.Of401("Invalid email or password.");
        }

        // Check account status (if account is disabled)
        if (!user.IsActive)
        {
            return Result<LoginResponse>.Of403("Account is disabled.");
        }

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();

        // Persist the Refresh Token to the database (store HASH, not plain)
        var tokenHash = secretHasher.Hash(refreshToken);
        user.RefreshToken = tokenHash;

        var refreshTokenDays = configuration.GetValue<int>("JwtSettings:RefreshTokenExpiryDays", 7);
        var expiresAt = DateTime.UtcNow.AddDays(refreshTokenDays);

        user.RefreshTokenExpiresAt = expiresAt;

        user.LastLoginAt = DateTime.UtcNow;

        userRepository.Update(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<LoginResponse>.Of200(
            "Login successful",
            new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken, // to Controller
                RefreshTokenExpiration = expiresAt,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role.ToString(),
                    OrganizationId = user.OrganizationId,
                },
            }
        );
    }
}
