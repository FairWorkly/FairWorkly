using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Domain.Auth.Interfaces;
using Microsoft.Extensions.Configuration;

using MediatR;

namespace FairWorkly.Application.Auth.Features.Refresh;

public class RefreshCommandHandler(
    IUserRepository userRepository,
    ISecretHasher secretHasher,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    IConfiguration configuration
) : IRequestHandler<RefreshCommand, RefreshResult>
{
    public async Task<RefreshResult> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        // Hash incoming plain token and lookup user
        var incomingHash = secretHasher.Hash(request.RefreshTokenPlain);
        var user = await userRepository.GetByRefreshTokenHashAsync(incomingHash, cancellationToken);
        if (user == null)
        {
            return new RefreshResult
            {
                FailureReason = RefreshFailureReason.InvalidToken
            };
        }

        if (!user.IsActive)
        {
            return new RefreshResult
            {
                FailureReason = RefreshFailureReason.AccountDisabled
            };
        }

        // Check expiry
        if (!user.RefreshTokenExpiresAt.HasValue || user.RefreshTokenExpiresAt.Value < DateTime.UtcNow)
        {
            return new RefreshResult
            {
                FailureReason = RefreshFailureReason.ExpiredToken
            };
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

        return new RefreshResult
        {
            Response = new RefreshResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshPlain,
                RefreshTokenExpiration = newExpires
            }
        };
    }
}
