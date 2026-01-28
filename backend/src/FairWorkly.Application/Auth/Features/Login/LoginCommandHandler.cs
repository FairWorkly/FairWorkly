using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Domain.Auth.Interfaces;
using FairWorkly.Domain.Common;
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
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user == null || !passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Result<LoginResponse>.Unauthorized("Invalid email or password.");
        }

        // Check account status (if account is disabled)
        if (!user.IsActive)
        {
            return Result<LoginResponse>.Forbidden("Account is disabled.");
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

        // return
        return Result<LoginResponse>.Success(
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
