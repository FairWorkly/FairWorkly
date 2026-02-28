using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FairWorkly.Infrastructure.Persistence;
using FairWorkly.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FairWorkly.IntegrationTests.Auth;

public class RegisterTests : AuthTestsBase
{
    public RegisterTests(CustomWebApplicationFactory factory)
        : base(factory) { }

    private static object CreateValidPayload(
        string? abnOverride = null,
        string? emailOverride = null
    )
    {
        var unique = Guid.NewGuid().ToString("N")[..8];
        var abnDigits = new Random().Next(10000000, 99999999);
        return new
        {
            companyName = $"Test Company {unique}",
            abn = abnOverride ?? $"100{abnDigits}",
            industryType = "Retail",
            addressLine1 = "123 Test St",
            suburb = "Melbourne",
            state = "VIC",
            postcode = "3000",
            contactEmail = $"contact-{unique}@test.com",
            email = emailOverride ?? $"owner-{unique}@test.com",
            password = "Test1234",
            firstName = "Owner",
            lastName = "User",
        };
    }

    #region 1.1 Validation Failures (400)

    [Fact]
    public async Task Register_EmptyFields_Returns400WithValidationErrors()
    {
        // Arrange
        var request = new
        {
            companyName = "",
            abn = "",
            industryType = "",
            addressLine1 = "",
            suburb = "",
            state = "",
            postcode = "",
            contactEmail = "",
            email = "",
            password = "",
            firstName = "",
            lastName = "",
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        root.GetProperty("code").GetInt32().Should().Be(400);

        var errors = root.GetProperty("data").GetProperty("errors");
        errors.GetArrayLength().Should().BeGreaterThanOrEqualTo(8);

        var errorFields = errors
            .EnumerateArray()
            .Select(e => e.GetProperty("field").GetString())
            .ToList();

        errorFields.Should().Contain("CompanyName");
        errorFields.Should().Contain("Abn");
        errorFields.Should().Contain("Email");
        errorFields.Should().Contain("Password");
        errorFields.Should().Contain("FirstName");
        errorFields.Should().Contain("LastName");
    }

    [Fact]
    public async Task Register_InvalidAbnFormat_Returns400()
    {
        // Arrange — ABN must be exactly 11 digits
        var request = new
        {
            companyName = "Test Co",
            abn = "123",
            industryType = "Retail",
            addressLine1 = "123 St",
            suburb = "Melbourne",
            state = "VIC",
            postcode = "3000",
            contactEmail = "c@test.com",
            email = "o@test.com",
            password = "Test1234",
            firstName = "A",
            lastName = "B",
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var errors = doc.RootElement.GetProperty("data").GetProperty("errors");
        errors
            .EnumerateArray()
            .Should()
            .Contain(e =>
                e.GetProperty("field").GetString() == "Abn"
                && e.GetProperty("message").GetString() == "ABN must be 11 digits."
            );
    }

    [Fact]
    public async Task Register_InvalidState_Returns400()
    {
        // Arrange
        var request = new
        {
            companyName = "Test Co",
            abn = "12345678901",
            industryType = "Retail",
            addressLine1 = "123 St",
            suburb = "Melbourne",
            state = "INVALID",
            postcode = "3000",
            contactEmail = "c@test.com",
            email = "o@test.com",
            password = "Test1234",
            firstName = "A",
            lastName = "B",
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WeakPassword_Returns400()
    {
        // Arrange — password without numbers
        var request = new
        {
            companyName = "Test Co",
            abn = "12345678901",
            industryType = "Retail",
            addressLine1 = "123 St",
            suburb = "Melbourne",
            state = "VIC",
            postcode = "3000",
            contactEmail = "c@test.com",
            email = "o@test.com",
            password = "abcdefgh",
            firstName = "A",
            lastName = "B",
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var errors = doc.RootElement.GetProperty("data").GetProperty("errors");
        errors
            .EnumerateArray()
            .Should()
            .Contain(e =>
                e.GetProperty("field").GetString() == "Password"
                && e.GetProperty("message").GetString()!.Contains("letters and numbers")
            );
    }

    #endregion

    #region 1.2 Duplicate ABN (409)

    [Fact]
    public async Task Register_DuplicateAbn_Returns409()
    {
        // Arrange — use the seed org's ABN
        var request = new
        {
            companyName = "Another Co",
            abn = "99999999901",
            industryType = "Retail",
            addressLine1 = "456 St",
            suburb = "Sydney",
            state = "NSW",
            postcode = "2000",
            contactEmail = "c2@test.com",
            email = $"dup-abn-{Guid.NewGuid():N}@test.com",
            password = "Test1234",
            firstName = "Dup",
            lastName = "Test",
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        doc.RootElement.GetProperty("code").GetInt32().Should().Be(409);
        doc.RootElement.GetProperty("msg").GetString().Should().Contain("ABN");
    }

    #endregion

    #region 1.3 Registration Success (201)

    [Fact]
    public async Task Register_ValidPayload_Returns201WithTokenAndCookie()
    {
        // Arrange
        var request = CreateValidPayload();

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", request);

        // Debug — print response body on failure
        var debugBody = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.Created, debugBody);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        root.GetProperty("code").GetInt32().Should().Be(201);
        root.GetProperty("msg").GetString().Should().Be("Registration successful");

        // Assert — Response body
        var data = root.GetProperty("data");

        data.TryGetProperty("accessToken", out var accessToken).Should().BeTrue();
        accessToken.GetString().Should().NotBeNullOrEmpty();

        // refreshToken should NOT be in body (HttpOnly cookie only)
        data.TryGetProperty("refreshToken", out _).Should().BeFalse();

        var user = data.GetProperty("user");
        user.GetProperty("email").GetString().Should().NotBeNullOrEmpty();
        user.GetProperty("firstName").GetString().Should().NotBeNullOrEmpty();
        user.GetProperty("lastName").GetString().Should().NotBeNullOrEmpty();
        user.GetProperty("role").GetString().Should().Be("Admin");
        user.TryGetProperty("organizationId", out _).Should().BeTrue();

        // Assert — Cookie
        response.Headers.TryGetValues("Set-Cookie", out var cookies).Should().BeTrue();
        var cookieString = string.Join("; ", cookies!);
        cookieString.Should().Contain("refreshToken=");
        cookieString.ToLowerInvariant().Should().Contain("httponly");
    }

    [Fact]
    public async Task Register_CreatesOrganizationAndUserInDb()
    {
        // Arrange
        var unique = Guid.NewGuid().ToString("N")[..8];
        var abnDigits = new Random().Next(10000000, 99999999);
        var abn = $"200{abnDigits}";
        var email = $"db-check-{unique}@test.com";
        var request = CreateValidPayload(abnOverride: abn, emailOverride: email);

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // Assert — verify DB state
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FairWorklyDbContext>();

        var org = await db.Organizations.FirstOrDefaultAsync(o => o.ABN == abn);
        org.Should().NotBeNull();
        org!.IsSubscriptionActive.Should().BeTrue();

        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
        user.Should().NotBeNull();
        user!.OrganizationId.Should().Be(org.Id);
        user.Role.Should().Be(Domain.Auth.Enums.UserRole.Admin);
        user.IsActive.Should().BeTrue();
        user.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Register_SameEmailDifferentOrg_Succeeds()
    {
        // Arrange — register first org
        var sharedEmail = $"multi-org-{Guid.NewGuid():N}@test.com";
        var first = CreateValidPayload(emailOverride: sharedEmail);
        var firstResponse = await Client.PostAsJsonAsync("/api/auth/register", first);
        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Act — register second org with same email, different ABN
        var second = CreateValidPayload(emailOverride: sharedEmail);
        var secondResponse = await Client.PostAsJsonAsync("/api/auth/register", second);

        // Assert — multi-tenancy: same email allowed in different orgs
        secondResponse.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    #endregion
}
