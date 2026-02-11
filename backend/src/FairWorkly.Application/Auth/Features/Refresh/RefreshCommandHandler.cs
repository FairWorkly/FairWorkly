using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Domain.Auth.Interfaces;
using FairWorkly.Domain.Common;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace FairWorkly.Application.Auth.Features.Refresh;

public class RefreshCommandHandler(
    IUserRepository userRepository,
    ISecretHasher secretHasher,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    IConfiguration configuration
) : IRequestHandler<RefreshCommand, Result<RefreshResponse>>
{
    public async Task<Result<RefreshResponse>> Handle(
        RefreshCommand request,
        CancellationToken cancellationToken
    )
    {
        // Hash incoming plain token and lookup user
        var incomingHash = secretHasher.Hash(request.RefreshTokenPlain);
        var user = await userRepository.GetByRefreshTokenHashAsync(incomingHash, cancellationToken);
        if (user == null)
        {
            return Result<RefreshResponse>.Unauthorized("Invalid refresh token.");
        }

        if (!user.IsActive)
        {
            return Result<RefreshResponse>.Forbidden("Account is disabled.");
        }

        // Check expiry
        if (
            !user.RefreshTokenExpiresAt.HasValue
            || user.RefreshTokenExpiresAt.Value < DateTime.UtcNow
        )
        {
            return Result<RefreshResponse>.Unauthorized("Refresh token expired.");
        }

        // Passed validation - rotate tokens
        var newAccessToken = tokenService.GenerateAccessToken(user);
        var newRefreshPlain = tokenService.GenerateRefreshToken();
        var newRefreshHash = secretHasher.Hash(newRefreshPlain);

        user.RefreshToken = newRefreshHash;
        var refreshDays = configuration.GetValue<int>("JwtSettings:RefreshTokenExpiryDays", 7);
        var newExpires = DateTime.UtcNow.AddDays(refreshDays);
        user.RefreshTokenExpiresAt = newExpires;
        user.LastLoginAt = DateTime.UtcNow;

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<RefreshResponse>.Success(
            new RefreshResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshPlain,
                RefreshTokenExpiration = newExpires,
            }
        );
    }
}
