using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FairWorkly.Application.Common.Interfaces;

namespace FairWorkly.API.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId =>
        _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? _httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

    public string? OrganizationId =>
        _httpContextAccessor.HttpContext?.User.FindFirst("orgId")?.Value;

    public string? Email =>
        _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value
        ?? _httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Email)?.Value;
}
