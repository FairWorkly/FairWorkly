using FairWorkly.Domain.Auth.Entities;

namespace FairWorkly.Application.Common.Interfaces;

public interface ITokenService
{
    // Generate short-lived Access Token (JWT)
    string GenerateAccessToken(User user, Guid? employeeId = null);

    // Generate long-lived Refresh Token (random string)
    string GenerateRefreshToken();
}
