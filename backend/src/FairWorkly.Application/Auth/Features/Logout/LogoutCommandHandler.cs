using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Domain.Auth.Interfaces;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Result;
using MediatR;

namespace FairWorkly.Application.Auth.Features.Logout;

public class LogoutCommandHandler(
    IUserRepository userRepository,
    ISecretHasher secretHasher,
    IUnitOfWork unitOfWork
) : IRequestHandler<LogoutCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken
    )
    {
        // Try to resolve user by id first
        Domain.Auth.Entities.User? user = null;

        if (request.UserId.HasValue)
        {
            user = await userRepository.GetByIdAsync(request.UserId.Value, cancellationToken);
        }

        // If not found by id, try by refresh token hash if provided
        if (user == null && !string.IsNullOrWhiteSpace(request.RefreshTokenPlain))
        {
            var hash = secretHasher.Hash(request.RefreshTokenPlain!);
            user = await userRepository.GetByRefreshTokenHashAsync(hash, cancellationToken);
        }

        if (user == null)
        {
            return Result<Unit>.Of404("Logout failed.");
        }

        // Clear persisted refresh token and expiry
        user.RefreshToken = null;
        user.RefreshTokenExpiresAt = null;
        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Of204();
    }
}
