using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FairWorkly.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace FairWorkly.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public string? UserId =>
        User?.FindFirstValue(JwtRegisteredClaimNames.Sub)
        ?? User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public string? OrganizationId =>
        User?.FindFirstValue("orgId");

    public string? Email =>
        User?.FindFirstValue(JwtRegisteredClaimNames.Email)
        ?? User?.FindFirstValue(ClaimTypes.Email);
}
