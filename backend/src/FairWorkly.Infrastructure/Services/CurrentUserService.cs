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

    public Guid? UserId
    {
        get
        {
            var sub = User?.FindFirstValue(JwtRegisteredClaimNames.Sub)
                      ?? User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(sub, out var id) ? id : null;
        }
    }

    public Guid? OrganizationId
    {
        get
        {
            var orgId = User?.FindFirstValue("orgId");
            return Guid.TryParse(orgId, out var id) ? id : null;
        }
    }

    public string? Email =>
        User?.FindFirstValue(JwtRegisteredClaimNames.Email)
        ?? User?.FindFirstValue(ClaimTypes.Email);

    public string? Role =>
        User?.FindFirstValue("role")
        ?? User?.FindFirstValue(ClaimTypes.Role);

    public Guid? EmployeeId
    {
        get
        {
            var employeeId = User?.FindFirstValue("employeeId");
            return Guid.TryParse(employeeId, out var id) ? id : null;
        }
    }
}
