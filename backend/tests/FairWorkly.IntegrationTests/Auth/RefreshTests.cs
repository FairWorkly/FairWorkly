using System.Net;
using System.Text.Json;
using FairWorkly.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace FairWorkly.IntegrationTests.Auth;

public class RefreshTests : AuthTestsBase
{
    public RefreshTests(CustomWebApplicationFactory factory)
        : base(factory) { }

    #region 2.1 No Cookie (401)

    [Fact]
    public async Task Refresh_NoCookie_Returns401()
    {
        // Arrange - Request without cookie
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/auth/refresh");

        // Act
        var response = await Client.SendAsync(request);

        // Assert - Status code is 401 Unauthorized
        // Note: ASP.NET Core returns a ProblemDetails body for Unauthorized()
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region 2.2 Invalid Token (401)

    [Fact]
    public async Task Refresh_InvalidToken_Returns401WithExactMessage()
    {
        // Arrange - Client with random invalid token
        var client = CreateClientWithRefreshCookie("randomInvalidToken123");

        // Act
        var response = await client.PostAsync("/api/auth/refresh", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        root.GetProperty("message").GetString().Should().Be("Invalid refresh token.");
    }

    #endregion

    #region 2.3 Expired Token (401)

    [Fact]
    public async Task Refresh_ExpiredToken_Returns401WithExactMessage()
    {
        // Arrange - Set up expired refresh token
        var expiredToken = await SetupExpiredRefreshTokenAsync();
        var client = CreateClientWithRefreshCookie(expiredToken);

        // Act
        var response = await client.PostAsync("/api/auth/refresh", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        root.GetProperty("message").GetString().Should().Be("Refresh token expired.");
    }

    #endregion

    #region 2.4 Account Disabled (403)

    [Fact]
    public async Task Refresh_DisabledAccount_Returns403WithExactMessage()
    {
        // Arrange - Set up refresh token for disabled user manually
        // First, we need to temporarily enable the user, get a refresh token, then disable

        // Get fresh factory scope to manipulate the database
        using var scope = Factory.Services.CreateScope();
        var db =
            scope.ServiceProvider.GetRequiredService<FairWorkly.Infrastructure.Persistence.FairWorklyDbContext>();
        var passwordHasher =
            scope.ServiceProvider.GetRequiredService<FairWorkly.Application.Common.Interfaces.IPasswordHasher>();
        var secretHasher =
            scope.ServiceProvider.GetRequiredService<FairWorkly.Application.Common.Interfaces.ISecretHasher>();

        // Find the disabled user and temporarily enable them
        var disabledUser = db.Set<FairWorkly.Domain.Auth.Entities.User>()
            .FirstOrDefault(u => u.Email == "disabled@example.com");

        disabledUser.Should().NotBeNull("seed user disabled@example.com must exist for this test");

        // Temporarily enable, set a refresh token, then disable again
        disabledUser!.IsActive = true;
        var refreshTokenPlain = "testRefreshTokenForDisabledUser";
        disabledUser.RefreshToken = secretHasher.Hash(refreshTokenPlain);
        disabledUser.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
        db.SaveChanges();

        // Now disable the user again
        disabledUser.IsActive = false;
        db.SaveChanges();

        // Create client with the refresh token
        var client = CreateClientWithRefreshCookie(refreshTokenPlain);

        // Act
        var response = await client.PostAsync("/api/auth/refresh", null);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = System.Text.Json.JsonDocument.Parse(json);
        var root = doc.RootElement;

        root.GetProperty("message").GetString().Should().Be("Account is disabled.");
    }

    #endregion

    #region 2.5 Success (200)

    [Fact]
    public async Task Refresh_ValidToken_Returns200WithNewAccessToken()
    {
        // Arrange - Get a valid refresh token by logging in first
        var refreshToken = await GetRefreshTokenFromCookieAsync();
        var client = CreateClientWithRefreshCookie(refreshToken);

        // Act
        var response = await client.PostAsync("/api/auth/refresh", null);

        // Assert - Status code
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert - Response body
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        root.TryGetProperty("accessToken", out var accessToken).Should().BeTrue();
        accessToken.GetString().Should().NotBeNullOrEmpty();

        // Assert - New cookie is set
        response.Headers.TryGetValues("Set-Cookie", out var cookies).Should().BeTrue();
        var cookieString = string.Join("; ", cookies!);
        cookieString.Should().Contain("refreshToken=");
        // Cookie attributes are case-insensitive per RFC 6265
        cookieString.ToLowerInvariant().Should().Contain("httponly");
    }

    #endregion
}
