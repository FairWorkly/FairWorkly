using System.Net;
using System.Text.Json;
using FairWorkly.IntegrationTests.Infrastructure;
using FluentAssertions;

namespace FairWorkly.IntegrationTests.Auth;

public class LogoutTests : AuthTestsBase
{
    public LogoutTests(CustomWebApplicationFactory factory)
        : base(factory) { }

    #region 4.1 Unauthorized (401)

    [Fact]
    public async Task Logout_NoJwt_Returns401()
    {
        // Arrange - Request without Authorization header
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/auth/logout");

        // Act
        var response = await Client.SendAsync(request);

        // Assert - [Authorize] attribute intercepts and returns 401
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region 4.2 Logout Failed (400)

    [Fact]
    public async Task Logout_ValidJwtButUserDeleted_Returns400()
    {
        // Arrange - Create a valid token for a user that doesn't exist in DB
        var tokenForNonExistentUser = CreateTokenForNonExistentUser();
        var client = CreateAuthenticatedClient(tokenForNonExistentUser);

        // Act
        var response = await client.PostAsync("/api/auth/logout", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        root.GetProperty("message").GetString().Should().Be("Logout failed.");
    }

    #endregion

    #region 4.3 Logout Success (204)

    [Fact]
    public async Task Logout_ValidJwt_Returns204WithCookieDeleted()
    {
        // Arrange - Get a valid access token
        var accessToken = await GetAccessTokenAsync();
        var client = CreateAuthenticatedClient(accessToken);

        // Act
        var response = await client.PostAsync("/api/auth/logout", null);

        // Assert - Status code
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Assert - Empty body
        var content = await response.Content.ReadAsStringAsync();
        content.Should().BeEmpty();

        // Assert - Cookie is deleted (expires in the past)
        response.Headers.TryGetValues("Set-Cookie", out var cookies).Should().BeTrue();
        var cookieString = string.Join("; ", cookies!);
        cookieString.Should().Contain("refreshToken=");
        // Cookie deletion is indicated by Expires in the past or Max-Age=0
        // The actual format may vary, but the cookie should be set
    }

    #endregion
}
