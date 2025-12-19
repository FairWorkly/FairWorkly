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
) : IRequestHandler<RefreshCommand, RefreshResponse?>
{
    public async Task<RefreshResponse?> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshTokenPlain)) return null;

        // Hash incoming plain token and lookup user
        var incomingHash = secretHasher.Hash(request.RefreshTokenPlain);
        var user = await userRepository.GetByRefreshTokenHashAsync(incomingHash, cancellationToken);
        if (user == null) return null;

        // Check expiry
        if (!user.RefreshTokenExpiresAt.HasValue || user.RefreshTokenExpiresAt.Value < DateTime.UtcNow)
        {
            return null;
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

        return new RefreshResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshPlain,
            RefreshTokenExpiration = newExpires
        };
    }
}
