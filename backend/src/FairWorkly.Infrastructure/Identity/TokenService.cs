using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Auth.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FairWorkly.Infrastructure.Identity;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly IDateTimeProvider _dateTimeProvider;

    public TokenService(IConfiguration configuration, IDateTimeProvider dateTimeProvider)
    {
        _configuration = configuration;
        _dateTimeProvider = dateTimeProvider;
    }

    public string GenerateAccessToken(User user, Guid? employeeId = null)
    {
        // Read JWT key parameters from configuration
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey =
            jwtSettings["Secret"]
            ?? throw new InvalidOperationException("JWT Secret is missing in config.");
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var expiryMinutes = jwtSettings.GetValue<int>("ExpiryMinutes", 15);

        // Prepare key and signing algorithm
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        if (!Enum.IsDefined(typeof(UserRole), user.Role))
        {
            throw new InvalidOperationException($"Invalid user role value: {(int)user.Role}");
        }

        // Assemble Claims (payload)
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), // User ID
            new Claim(JwtRegisteredClaimNames.Email, user.Email), // Email
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Token unique ID
            // --- decision fields ---
            new Claim("orgId", user.OrganizationId.ToString()), // [Decision 1] Tenant isolation
            new Claim("role", user.Role.ToString()), // [Decision 2] Role as string
        };

        // [Decision 3] If EmployeeId exists, include it
        if (employeeId.HasValue)
        {
            claims.Add(new Claim("employeeId", employeeId.Value.ToString()));
        }

        // Generate Token object
        var currentTime = _dateTimeProvider.UtcNow.UtcDateTime;
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: currentTime,
            expires: currentTime.AddMinutes(expiryMinutes),
            signingCredentials: creds
        );

        // Serialize to string
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        // Generate a 32-byte high-entropy random number
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
