using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace FairWorkly.IntegrationTests.Infrastructure;

public abstract class AuthTestsBase : IClassFixture<CustomWebApplicationFactory>
{
    protected readonly HttpClient Client;
    protected readonly CustomWebApplicationFactory Factory;

    protected AuthTestsBase(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    /// <summary>
    /// Login and get access token
    /// </summary>
    protected async Task<string> GetAccessTokenAsync(
        string email = "test@example.com",
        string password = "TestPassword123"
    )
    {
        var response = await Client.PostAsJsonAsync("/api/auth/login", new { email, password });
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("accessToken").GetString()!;
    }

    /// <summary>
    /// Login and get refresh token from cookie
    /// </summary>
    protected async Task<string> GetRefreshTokenFromCookieAsync(
        string email = "test@example.com",
        string password = "TestPassword123"
    )
    {
        var response = await Client.PostAsJsonAsync("/api/auth/login", new { email, password });
        response.EnsureSuccessStatusCode();

        if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
        {
            var refreshCookie = cookies.FirstOrDefault(c => c.StartsWith("refreshToken="));
            if (refreshCookie != null)
            {
                // Extract token value from "refreshToken=xxx; HttpOnly; ..."
                var tokenPart = refreshCookie.Split(';')[0];
                return tokenPart.Substring("refreshToken=".Length);
            }
        }

        throw new InvalidOperationException("No refresh token cookie found");
    }

    /// <summary>
    /// Create an expired JWT token for testing
    /// </summary>
    protected string CreateExpiredJwtToken()
    {
        using var scope = Factory.Services.CreateScope();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        var jwtSettings = config.GetSection("JwtSettings");
        var secretKey = jwtSettings["Secret"] ?? "TestSecretKeyForIntegrationTests123456";
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Get the test user's ID
        var db = scope.ServiceProvider.GetRequiredService<FairWorklyDbContext>();
        var testUser = db.Set<User>().FirstOrDefault(u => u.Email == "test@example.com");

        var claims = new List<Claim>
        {
            new Claim(
                JwtRegisteredClaimNames.Sub,
                testUser?.Id.ToString() ?? Guid.NewGuid().ToString()
            ),
            new Claim(JwtRegisteredClaimNames.Email, "test@example.com"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("orgId", testUser?.OrganizationId.ToString() ?? Guid.NewGuid().ToString()),
            new Claim("role", "Admin"),
        };

        // Create a token that expired 1 hour ago
        var expiredTime = DateTime.UtcNow.AddHours(-1);
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: expiredTime.AddMinutes(-15),
            expires: expiredTime,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Create a valid JWT token for a user that doesn't exist in the database
    /// </summary>
    protected string CreateTokenForNonExistentUser()
    {
        using var scope = Factory.Services.CreateScope();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        var jwtSettings = config.GetSection("JwtSettings");
        var secretKey = jwtSettings["Secret"] ?? "TestSecretKeyForIntegrationTests123456";
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var nonExistentUserId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, nonExistentUserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, "nonexistent@example.com"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("orgId", Guid.NewGuid().ToString()),
            new Claim("role", "Admin"),
        };

        var currentTime = DateTime.UtcNow;
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: currentTime,
            expires: currentTime.AddMinutes(15),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Set up a user with an expired refresh token for testing
    /// </summary>
    protected async Task<string> SetupExpiredRefreshTokenAsync()
    {
        // First login to get a valid refresh token
        var response = await Client.PostAsJsonAsync(
            "/api/auth/login",
            new { email = "test@example.com", password = "TestPassword123" }
        );
        response.EnsureSuccessStatusCode();

        // Get the refresh token
        string? refreshToken = null;
        if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
        {
            var refreshCookie = cookies.FirstOrDefault(c => c.StartsWith("refreshToken="));
            if (refreshCookie != null)
            {
                var tokenPart = refreshCookie.Split(';')[0];
                refreshToken = tokenPart.Substring("refreshToken=".Length);
            }
        }

        if (refreshToken == null)
        {
            throw new InvalidOperationException("Failed to get refresh token");
        }

        // Update the user's refresh token expiry to be in the past
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FairWorklyDbContext>();
        var user = db.Set<User>().FirstOrDefault(u => u.Email == "test@example.com");
        if (user != null)
        {
            user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(-1);
            db.SaveChanges();
        }

        return refreshToken;
    }

    /// <summary>
    /// Create HTTP client with Authorization header
    /// </summary>
    protected HttpClient CreateAuthenticatedClient(string token)
    {
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    /// <summary>
    /// Create HTTP client with refresh token cookie
    /// </summary>
    protected HttpClient CreateClientWithRefreshCookie(string refreshToken)
    {
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Add("Cookie", $"refreshToken={refreshToken}");
        return client;
    }
}
