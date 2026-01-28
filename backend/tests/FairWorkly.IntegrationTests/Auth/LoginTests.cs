using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FairWorkly.IntegrationTests.Infrastructure;
using FluentAssertions;

namespace FairWorkly.IntegrationTests.Auth;

public class LoginTests : AuthTestsBase
{
    public LoginTests(CustomWebApplicationFactory factory)
        : base(factory) { }

    #region 1.1 FluentValidation Failures (400)

    [Fact]
    public async Task Login_EmptyEmailAndPassword_Returns400WithExactProblemDetails()
    {
        // Arrange
        var request = new { email = "", password = "" };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", request);

        // Assert - Status code
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Assert - Content-Type
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");

        // Assert - Complete ProblemDetails structure
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        // Note: GlobalExceptionHandler doesn't set "type" property
        root.GetProperty("title").GetString().Should().Be("Validation Failed");
        root.GetProperty("status").GetInt32().Should().Be(400);
        root.GetProperty("detail")
            .GetString()
            .Should()
            .Be("One or more validation errors occurred.");
        root.GetProperty("instance").GetString().Should().Be("/api/auth/login");

        // Assert - Exact errors structure (in ProblemDetails extensions)
        var errors = root.GetProperty("errors");

        // Email error
        var emailErrors = errors
            .GetProperty("Email")
            .EnumerateArray()
            .Select(e => e.GetString())
            .ToList();
        emailErrors.Should().Contain("Email is required.");

        // Password error
        var passwordErrors = errors
            .GetProperty("Password")
            .EnumerateArray()
            .Select(e => e.GetString())
            .ToList();
        passwordErrors.Should().Contain("Password is required.");
    }

    [Fact]
    public async Task Login_InvalidEmailFormat_Returns400WithEmailError()
    {
        // Arrange
        var request = new { email = "invalid", password = "12345678" };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        root.GetProperty("title").GetString().Should().Be("Validation Failed");

        var errors = root.GetProperty("errors");
        var emailErrors = errors
            .GetProperty("Email")
            .EnumerateArray()
            .Select(e => e.GetString())
            .ToList();
        emailErrors.Should().Contain("A valid email is required.");
    }

    [Fact]
    public async Task Login_ShortPassword_Returns400WithPasswordError()
    {
        // Arrange
        var request = new { email = "test@example.com", password = "short" };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        root.GetProperty("title").GetString().Should().Be("Validation Failed");

        var errors = root.GetProperty("errors");
        var passwordErrors = errors
            .GetProperty("Password")
            .EnumerateArray()
            .Select(e => e.GetString())
            .ToList();
        passwordErrors.Should().Contain("Password must be at least 8 characters.");
    }

    #endregion

    #region 1.2 Invalid Credentials (401)

    [Fact]
    public async Task Login_NonExistentEmail_Returns401WithExactMessage()
    {
        // Arrange
        var request = new { email = "nonexistent@example.com", password = "wrongpassword123" };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        root.GetProperty("message").GetString().Should().Be("Invalid email or password.");
    }

    [Fact]
    public async Task Login_WrongPassword_Returns401WithExactMessage()
    {
        // Arrange
        var request = new { email = "test@example.com", password = "wrongpassword123" };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        root.GetProperty("message").GetString().Should().Be("Invalid email or password.");
    }

    #endregion

    #region 1.3 Account Disabled (403)

    [Fact]
    public async Task Login_DisabledAccount_Returns403WithExactMessage()
    {
        // Arrange - Using the pre-seeded disabled account
        var request = new { email = "disabled@example.com", password = "TestPassword123" };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", request);

        // Assert - Note: 403, not 401
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        root.GetProperty("message").GetString().Should().Be("Account is disabled.");
    }

    #endregion

    #region 1.4 Login Success (200)

    [Fact]
    public async Task Login_ValidCredentials_Returns200WithTokenAndCookie()
    {
        // Arrange
        var request = new { email = "test@example.com", password = "TestPassword123" };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", request);

        // Assert - Status code
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert - Response body structure
        // Note: refreshToken and refreshTokenExpiration are [JsonIgnore] and only set as cookie
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        root.TryGetProperty("accessToken", out var accessToken).Should().BeTrue();
        accessToken.GetString().Should().NotBeNullOrEmpty();

        // Verify refreshToken is NOT in body (it's HttpOnly cookie only)
        root.TryGetProperty("refreshToken", out _).Should().BeFalse();
        root.TryGetProperty("refreshTokenExpiration", out _).Should().BeFalse();

        root.TryGetProperty("user", out var user).Should().BeTrue();
        user.TryGetProperty("id", out _).Should().BeTrue();
        user.TryGetProperty("email", out _).Should().BeTrue();
        user.TryGetProperty("firstName", out _).Should().BeTrue();
        user.TryGetProperty("lastName", out _).Should().BeTrue();
        user.TryGetProperty("role", out _).Should().BeTrue();
        user.TryGetProperty("organizationId", out _).Should().BeTrue();

        // Assert - Cookie (refreshToken is only in HttpOnly cookie)
        response.Headers.TryGetValues("Set-Cookie", out var cookies).Should().BeTrue();
        var cookieString = string.Join("; ", cookies!);
        cookieString.Should().Contain("refreshToken=");
        // Cookie attributes are case-insensitive per RFC 6265
        cookieString.ToLowerInvariant().Should().Contain("httponly");
        cookieString.ToLowerInvariant().Should().Contain("path=/");
    }

    #endregion
}
