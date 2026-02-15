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
    public async Task Login_EmptyEmailAndPassword_Returns400WithValidationErrors()
    {
        // Arrange
        var request = new { email = "", password = "" };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", request);

        // Assert - Status code
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Assert - Content-Type
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");

        // Assert - Unified response format { code, msg, data: { errors } }
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        root.GetProperty("code").GetInt32().Should().Be(400);
        root.GetProperty("msg").GetString().Should().NotBeNullOrEmpty();

        // Assert - Errors array in data.errors
        var errors = root.GetProperty("data").GetProperty("errors");
        errors.GetArrayLength().Should().BeGreaterThan(0);

        // Collect all error fields and messages
        var errorList = errors
            .EnumerateArray()
            .Select(e => new
            {
                Field = e.GetProperty("field").GetString(),
                Message = e.GetProperty("message").GetString(),
            })
            .ToList();

        // Email errors
        errorList.Should().Contain(e => e.Field == "Email" && e.Message == "Email is required.");

        // Password errors
        errorList
            .Should()
            .Contain(e => e.Field == "Password" && e.Message == "Password is required.");
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

        root.GetProperty("code").GetInt32().Should().Be(400);

        var errors = root.GetProperty("data").GetProperty("errors");
        var errorList = errors
            .EnumerateArray()
            .Select(e => new
            {
                Field = e.GetProperty("field").GetString(),
                Message = e.GetProperty("message").GetString(),
            })
            .ToList();

        errorList
            .Should()
            .Contain(e => e.Field == "Email" && e.Message == "A valid email is required.");
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

        root.GetProperty("code").GetInt32().Should().Be(400);

        var errors = root.GetProperty("data").GetProperty("errors");
        var errorList = errors
            .EnumerateArray()
            .Select(e => new
            {
                Field = e.GetProperty("field").GetString(),
                Message = e.GetProperty("message").GetString(),
            })
            .ToList();

        errorList
            .Should()
            .Contain(e =>
                e.Field == "Password" && e.Message == "Password must be at least 8 characters."
            );
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

        root.GetProperty("msg").GetString().Should().Be("Invalid email or password.");
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

        root.GetProperty("msg").GetString().Should().Be("Invalid email or password.");
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

        root.GetProperty("msg").GetString().Should().Be("Account is disabled.");
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

        // Assert - Unified response format { code, msg, data }
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        root.GetProperty("code").GetInt32().Should().Be(200);
        root.GetProperty("msg").GetString().Should().Be("Login successful");

        var data = root.GetProperty("data");

        data.TryGetProperty("accessToken", out var accessToken).Should().BeTrue();
        accessToken.GetString().Should().NotBeNullOrEmpty();

        // Verify refreshToken is NOT in body (it's HttpOnly cookie only, [JsonIgnore])
        data.TryGetProperty("refreshToken", out _).Should().BeFalse();
        data.TryGetProperty("refreshTokenExpiration", out _).Should().BeFalse();

        data.TryGetProperty("user", out var user).Should().BeTrue();
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
