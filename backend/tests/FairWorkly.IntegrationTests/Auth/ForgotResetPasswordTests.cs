using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Auth.Enums;
using FairWorkly.Infrastructure.Persistence;
using FairWorkly.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FairWorkly.IntegrationTests.Auth;

public class ForgotResetPasswordTests : AuthTestsBase
{
    public ForgotResetPasswordTests(CustomWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task ForgotPassword_ShouldReturnSuccess_AndSetResetToken_ForKnownEmail()
    {
        var email = $"forgot_{Guid.NewGuid()}@example.com";
        await CreateUserAsync(email, "OldPassword123");

        var response = await Client.PostAsJsonAsync(
            "/api/auth/forgot-password",
            new { Email = email }
        );

        response.EnsureSuccessStatusCode();

        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
        Assert.Equal(200, json.GetProperty("code").GetInt32());
        Assert.Equal("Password reset requested", json.GetProperty("msg").GetString());

        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FairWorklyDbContext>();
        var user = await db.Users.FirstAsync(u => u.Email == email);

        Assert.NotNull(user.PasswordResetToken);
        Assert.NotNull(user.PasswordResetTokenExpiry);
        Assert.True(user.PasswordResetTokenExpiry > DateTime.UtcNow);
    }

    [Fact]
    public async Task ForgotPassword_ShouldReturnSuccess_ForUnknownEmail()
    {
        var response = await Client.PostAsJsonAsync(
            "/api/auth/forgot-password",
            new { Email = $"unknown_{Guid.NewGuid()}@example.com" }
        );

        response.EnsureSuccessStatusCode();

        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
        Assert.Equal(200, json.GetProperty("code").GetInt32());
        Assert.Equal("Password reset requested", json.GetProperty("msg").GetString());
    }

    [Fact]
    public async Task ValidateResetPasswordToken_ShouldSucceed_ForValidToken()
    {
        var plainToken = $"reset-validate-{Guid.NewGuid()}";
        await CreateUserAsync(
            $"validate_{Guid.NewGuid()}@example.com",
            "OldPassword123",
            plainToken,
            DateTime.UtcNow.AddMinutes(30)
        );

        var response = await Client.GetAsync(
            $"/api/auth/reset-password/validate?token={Uri.EscapeDataString(plainToken)}"
        );

        response.EnsureSuccessStatusCode();

        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
        Assert.Equal(200, json.GetProperty("code").GetInt32());
        Assert.True(json.GetProperty("data").GetBoolean());
    }

    [Fact]
    public async Task ValidateResetPasswordToken_ShouldFail_WhenExpired()
    {
        var plainToken = $"reset-expired-{Guid.NewGuid()}";
        await CreateUserAsync(
            $"expired_{Guid.NewGuid()}@example.com",
            "OldPassword123",
            plainToken,
            DateTime.UtcNow.AddMinutes(-5)
        );

        var response = await Client.GetAsync(
            $"/api/auth/reset-password/validate?token={Uri.EscapeDataString(plainToken)}"
        );

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
        Assert.Contains("expired", json.GetProperty("msg").GetString()!.ToLowerInvariant());
    }

    [Fact]
    public async Task ResetPassword_ShouldSucceed_AndAllowLoginWithNewPassword()
    {
        var email = $"reset_{Guid.NewGuid()}@example.com";
        var oldPassword = "OldPassword123";
        var newPassword = "NewPassword123";
        var plainToken = $"reset-success-{Guid.NewGuid()}";

        await CreateUserAsync(
            email,
            oldPassword,
            plainToken,
            DateTime.UtcNow.AddMinutes(30),
            includeRefreshToken: true
        );

        var resetResponse = await Client.PostAsJsonAsync(
            "/api/auth/reset-password",
            new { Token = plainToken, Password = newPassword }
        );

        resetResponse.EnsureSuccessStatusCode();

        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<FairWorklyDbContext>();
            var user = await db.Users.FirstAsync(u => u.Email == email);

            Assert.Null(user.PasswordResetToken);
            Assert.Null(user.PasswordResetTokenExpiry);
            Assert.Null(user.RefreshToken);
            Assert.Null(user.RefreshTokenExpiresAt);
        }

        var oldLoginResponse = await Client.PostAsJsonAsync(
            "/api/auth/login",
            new { Email = email, Password = oldPassword }
        );
        Assert.Equal(HttpStatusCode.Unauthorized, oldLoginResponse.StatusCode);

        var newLoginResponse = await Client.PostAsJsonAsync(
            "/api/auth/login",
            new { Email = email, Password = newPassword }
        );
        newLoginResponse.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task ResetPassword_ShouldFail_WithInvalidToken()
    {
        var response = await Client.PostAsJsonAsync(
            "/api/auth/reset-password",
            new { Token = $"invalid-{Guid.NewGuid()}", Password = "NewPassword123" }
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ResetPassword_ShouldFail_WhenTokenExpired()
    {
        var plainToken = $"reset-expired-submit-{Guid.NewGuid()}";
        await CreateUserAsync(
            $"expired_submit_{Guid.NewGuid()}@example.com",
            "OldPassword123",
            plainToken,
            DateTime.UtcNow.AddMinutes(-1)
        );

        var response = await Client.PostAsJsonAsync(
            "/api/auth/reset-password",
            new { Token = plainToken, Password = "NewPassword123" }
        );

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task ResetPassword_ShouldFail_WhenPasswordDoesNotMeetPolicy()
    {
        var response = await Client.PostAsJsonAsync(
            "/api/auth/reset-password",
            new { Token = $"token-{Guid.NewGuid()}", Password = "short" }
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task CreateUserAsync(
        string email,
        string password,
        string? resetTokenPlain = null,
        DateTime? resetTokenExpiry = null,
        bool includeRefreshToken = false
    )
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FairWorklyDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var secretHasher = scope.ServiceProvider.GetRequiredService<ISecretHasher>();
        var adminUser = await db.Users.FirstAsync(u => u.Email == "test@example.com");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            FirstName = "Reset",
            LastName = "User",
            Role = UserRole.Admin,
            IsActive = true,
            OrganizationId = adminUser.OrganizationId,
            PasswordHash = passwordHasher.Hash(password),
            PasswordResetToken =
                resetTokenPlain == null ? null : secretHasher.Hash(resetTokenPlain),
            PasswordResetTokenExpiry = resetTokenExpiry,
            RefreshToken = includeRefreshToken
                ? secretHasher.Hash($"refresh-{Guid.NewGuid()}")
                : null,
            RefreshTokenExpiresAt = includeRefreshToken ? DateTime.UtcNow.AddDays(7) : null,
            CreatedAt = DateTime.UtcNow,
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();
    }
}
