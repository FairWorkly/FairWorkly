using System.Net;
using System.Text.Json;
using FairWorkly.IntegrationTests.Infrastructure;
using FluentAssertions;

namespace FairWorkly.IntegrationTests.Auth;

public class MeTests : AuthTestsBase
{
    public MeTests(CustomWebApplicationFactory factory)
        : base(factory) { }

    #region 3.1 No Token (401)

    [Fact]
    public async Task Me_NoAuthorizationHeader_Returns401()
    {
        // Arrange - Request without Authorization header
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/auth/me");

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region 3.2 Invalid Token Format (401)

    [Fact]
    public async Task Me_InvalidTokenFormat_Returns401()
    {
        // Arrange - Client with invalid token
        var client = CreateAuthenticatedClient("invalidToken123");

        // Act
        var response = await client.GetAsync("/api/auth/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region 3.3 Expired Token (401)

    [Fact]
    public async Task Me_ExpiredToken_Returns401()
    {
        // Arrange - Create an expired JWT token
        var expiredToken = CreateExpiredJwtToken();
        var client = CreateAuthenticatedClient(expiredToken);

        // Act
        var response = await client.GetAsync("/api/auth/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region 3.4 User Not Found (404)

    [Fact]
    public async Task Me_UserNotFoundInDatabase_Returns404()
    {
        // Arrange - Create a valid token for a user that doesn't exist
        var tokenForNonExistentUser = CreateTokenForNonExistentUser();
        var client = CreateAuthenticatedClient(tokenForNonExistentUser);

        // Act
        var response = await client.GetAsync("/api/auth/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region 3.5 Success (200)

    [Fact]
    public async Task Me_ValidToken_Returns200WithUserInfo()
    {
        // Arrange - Get a valid access token
        var accessToken = await GetAccessTokenAsync();
        var client = CreateAuthenticatedClient(accessToken);

        // Act
        var response = await client.GetAsync("/api/auth/me");

        // Assert - Status code
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert - Response body structure
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        root.TryGetProperty("id", out _).Should().BeTrue();
        root.TryGetProperty("email", out var email).Should().BeTrue();
        email.GetString().Should().Be("test@example.com");
        root.TryGetProperty("firstName", out var firstName).Should().BeTrue();
        firstName.GetString().Should().Be("Test");
        root.TryGetProperty("lastName", out var lastName).Should().BeTrue();
        lastName.GetString().Should().Be("User");
        root.TryGetProperty("role", out _).Should().BeTrue();
        root.TryGetProperty("organizationId", out _).Should().BeTrue();
    }

    #endregion
}
