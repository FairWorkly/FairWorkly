using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Auth.Enums;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Infrastructure.Persistence;
using FairWorkly.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FairWorkly.IntegrationTests.Settings;

public class InvitationTests : AuthTestsBase
{
    public InvitationTests(CustomWebApplicationFactory factory)
        : base(factory) { }

    // ───────────────────────── Invite Team Member ─────────────────────────

    [Fact]
    public async Task InviteTeamMember_ShouldSucceed_WhenAdmin()
    {
        // Arrange
        var accessToken = await GetAccessTokenAsync();
        var client = CreateAuthenticatedClient(accessToken);
        var uniqueEmail = $"invite_{Guid.NewGuid()}@example.com";

        var request = new
        {
            Email = uniqueEmail,
            FirstName = "New",
            LastName = "Member",
            Role = "Manager",
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/settings/team/invite", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
        Assert.Equal(201, json.GetProperty("code").GetInt32());

        var data = json.GetProperty("data");
        Assert.NotEqual(Guid.Empty, data.GetProperty("userId").GetGuid());
        Assert.Contains("accept-invite?token=", data.GetProperty("inviteLink").GetString());

        // Verify DB state
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FairWorklyDbContext>();
        var createdUser = await db.Users.FirstOrDefaultAsync(u => u.Email == uniqueEmail);

        Assert.NotNull(createdUser);
        Assert.Equal(UserRole.Manager, createdUser.Role);
        Assert.False(createdUser.IsActive);
        Assert.Equal(InvitationStatus.Pending, createdUser.InvitationStatus);
        Assert.NotNull(createdUser.InvitationToken);
        Assert.NotNull(createdUser.InvitationTokenExpiry);
        Assert.True(createdUser.InvitationTokenExpiry > DateTime.UtcNow);
    }

    [Fact]
    public async Task InviteTeamMember_ShouldFail_WhenDuplicateEmail()
    {
        // Arrange
        var accessToken = await GetAccessTokenAsync();
        var client = CreateAuthenticatedClient(accessToken);

        // test@example.com already exists in the seeded data
        var request = new
        {
            Email = "test@example.com",
            FirstName = "Duplicate",
            LastName = "User",
            Role = "Manager",
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/settings/team/invite", request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task InviteTeamMember_ShouldFail_WhenInvalidRole()
    {
        // Arrange
        var accessToken = await GetAccessTokenAsync();
        var client = CreateAuthenticatedClient(accessToken);

        var request = new
        {
            Email = $"invalid_role_{Guid.NewGuid()}@example.com",
            FirstName = "Test",
            LastName = "User",
            Role = "SuperAdmin", // Invalid role
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/settings/team/invite", request);

        // Assert — validation returns 400
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task InviteTeamMember_ShouldFail_WhenMissingRequiredFields()
    {
        // Arrange
        var accessToken = await GetAccessTokenAsync();
        var client = CreateAuthenticatedClient(accessToken);

        var request = new
        {
            Email = "",
            FirstName = "",
            LastName = "",
            Role = "",
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/settings/team/invite", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task InviteTeamMember_ShouldReturnUnauthorized_WhenNotLoggedIn()
    {
        // Arrange
        var client = Factory.CreateClient();

        var request = new
        {
            Email = "noauth@example.com",
            FirstName = "No",
            LastName = "Auth",
            Role = "Manager",
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/settings/team/invite", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // ───────────────────────── Accept Invitation ─────────────────────────

    [Fact]
    public async Task AcceptInvitation_ShouldSucceed_WithValidToken()
    {
        // Arrange — seed a pending user with a known token
        var plainToken = $"test-accept-token-{Guid.NewGuid()}";
        var invitedEmail = $"accept_{Guid.NewGuid()}@example.com";

        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<FairWorklyDbContext>();
            var secretHasher = scope.ServiceProvider.GetRequiredService<ISecretHasher>();
            var adminUser = await db.Users.FirstAsync(u => u.Email == "test@example.com");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = invitedEmail,
                FirstName = "Accept",
                LastName = "Test",
                Role = UserRole.Manager,
                OrganizationId = adminUser.OrganizationId,
                IsActive = false,
                InvitationStatus = InvitationStatus.Pending,
                InvitationToken = secretHasher.Hash(plainToken),
                InvitationTokenExpiry = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();
        }

        var client = Factory.CreateClient(); // Anonymous — no auth needed

        var request = new { Token = plainToken, Password = "NewSecurePassword123" };

        // Act
        var response = await client.PostAsJsonAsync("/api/invite/accept", request);

        // Assert
        response.EnsureSuccessStatusCode();

        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
        Assert.Equal(200, json.GetProperty("code").GetInt32());

        var data = json.GetProperty("data");
        Assert.Equal(invitedEmail, data.GetProperty("email").GetString());
        Assert.Equal("Accept Test", data.GetProperty("fullName").GetString());

        // Verify DB state
        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<FairWorklyDbContext>();
            var user = await db.Users.FirstAsync(u => u.Email == invitedEmail);

            Assert.True(user.IsActive);
            Assert.Equal(InvitationStatus.Accepted, user.InvitationStatus);
            Assert.Null(user.InvitationToken); // Token cleared
            Assert.Null(user.InvitationTokenExpiry);
            Assert.NotNull(user.PasswordHash);
        }
    }

    [Fact]
    public async Task AcceptInvitation_ShouldFail_WithInvalidToken()
    {
        // Arrange
        var client = Factory.CreateClient();

        var request = new { Token = "this-token-does-not-exist", Password = "SomePassword123" };

        // Act
        var response = await client.PostAsJsonAsync("/api/invite/accept", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AcceptInvitation_ShouldFail_WhenTokenExpired()
    {
        // Arrange — seed a user with an expired token
        var plainToken = $"test-expired-token-{Guid.NewGuid()}";

        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<FairWorklyDbContext>();
            var secretHasher = scope.ServiceProvider.GetRequiredService<ISecretHasher>();
            var adminUser = await db.Users.FirstAsync(u => u.Email == "test@example.com");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = $"expired_{Guid.NewGuid()}@example.com",
                FirstName = "Expired",
                LastName = "Token",
                Role = UserRole.Manager,
                OrganizationId = adminUser.OrganizationId,
                IsActive = false,
                InvitationStatus = InvitationStatus.Pending,
                InvitationToken = secretHasher.Hash(plainToken),
                InvitationTokenExpiry = DateTime.UtcNow.AddDays(-1), // Already expired
                CreatedAt = DateTime.UtcNow,
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();
        }

        var client = Factory.CreateClient();

        var request = new { Token = plainToken, Password = "SomePassword123" };

        // Act
        var response = await client.PostAsJsonAsync("/api/invite/accept", request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task AcceptInvitation_ShouldFail_WhenAlreadyAccepted()
    {
        // Arrange — seed a user that already accepted
        var plainToken = $"test-accepted-token-{Guid.NewGuid()}";

        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<FairWorklyDbContext>();
            var secretHasher = scope.ServiceProvider.GetRequiredService<ISecretHasher>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
            var adminUser = await db.Users.FirstAsync(u => u.Email == "test@example.com");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = $"already_accepted_{Guid.NewGuid()}@example.com",
                FirstName = "Already",
                LastName = "Accepted",
                Role = UserRole.Manager,
                OrganizationId = adminUser.OrganizationId,
                IsActive = true,
                InvitationStatus = InvitationStatus.Accepted, // Already accepted
                InvitationToken = secretHasher.Hash(plainToken), // Token still in DB
                InvitationTokenExpiry = DateTime.UtcNow.AddDays(7),
                PasswordHash = passwordHasher.Hash("ExistingPassword123"),
                CreatedAt = DateTime.UtcNow,
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();
        }

        var client = Factory.CreateClient();

        var request = new { Token = plainToken, Password = "NewPassword123" };

        // Act
        var response = await client.PostAsJsonAsync("/api/invite/accept", request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task AcceptInvitation_ShouldFail_WhenPasswordTooShort()
    {
        // Arrange
        var client = Factory.CreateClient();

        var request = new
        {
            Token = "some-token",
            Password = "short", // Less than 8 chars
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/invite/accept", request);

        // Assert — validation returns 400
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ───────────────────────── Resend Invitation ─────────────────────────

    [Fact]
    public async Task ResendInvitation_ShouldSucceed_WhenPendingUser()
    {
        // Arrange — seed a pending user
        var accessToken = await GetAccessTokenAsync();
        var client = CreateAuthenticatedClient(accessToken);
        var pendingUserId = Guid.NewGuid();
        string oldTokenHash;

        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<FairWorklyDbContext>();
            var secretHasher = scope.ServiceProvider.GetRequiredService<ISecretHasher>();
            var adminUser = await db.Users.FirstAsync(u => u.Email == "test@example.com");

            oldTokenHash = secretHasher.Hash("old-token");
            var user = new User
            {
                Id = pendingUserId,
                Email = $"resend_{Guid.NewGuid()}@example.com",
                FirstName = "Resend",
                LastName = "Test",
                Role = UserRole.Manager,
                OrganizationId = adminUser.OrganizationId,
                IsActive = false,
                InvitationStatus = InvitationStatus.Pending,
                InvitationToken = oldTokenHash,
                InvitationTokenExpiry = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();
        }

        // Act
        var response = await client.PostAsync(
            $"/api/settings/team/{pendingUserId}/resend-invite",
            null
        );

        // Assert
        response.EnsureSuccessStatusCode();

        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
        Assert.Equal(200, json.GetProperty("code").GetInt32());
        Assert.Contains(
            "accept-invite?token=",
            json.GetProperty("data").GetProperty("inviteLink").GetString()
        );

        // Verify token was rotated in DB (new token must differ from the original)
        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<FairWorklyDbContext>();
            var user = await db.Users.FindAsync(pendingUserId);
            Assert.NotNull(user!.InvitationToken);
            Assert.NotEqual(oldTokenHash, user.InvitationToken);
            Assert.True(user.InvitationTokenExpiry > DateTime.UtcNow);
        }
    }

    [Fact]
    public async Task ResendInvitation_ShouldFail_WhenUserAlreadyAccepted()
    {
        // Arrange — seed a user who already accepted
        var accessToken = await GetAccessTokenAsync();
        var client = CreateAuthenticatedClient(accessToken);
        var acceptedUserId = Guid.NewGuid();

        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<FairWorklyDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
            var adminUser = await db.Users.FirstAsync(u => u.Email == "test@example.com");

            var user = new User
            {
                Id = acceptedUserId,
                Email = $"resend_accepted_{Guid.NewGuid()}@example.com",
                FirstName = "Already",
                LastName = "Active",
                Role = UserRole.Manager,
                OrganizationId = adminUser.OrganizationId,
                IsActive = true,
                InvitationStatus = InvitationStatus.Accepted,
                PasswordHash = passwordHasher.Hash("ExistingPassword123"),
                CreatedAt = DateTime.UtcNow,
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();
        }

        // Act
        var response = await client.PostAsync(
            $"/api/settings/team/{acceptedUserId}/resend-invite",
            null
        );

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task ResendInvitation_ShouldFail_WhenUserNotFound()
    {
        // Arrange
        var accessToken = await GetAccessTokenAsync();
        var client = CreateAuthenticatedClient(accessToken);
        var nonExistentUserId = Guid.NewGuid();

        // Act
        var response = await client.PostAsync(
            $"/api/settings/team/{nonExistentUserId}/resend-invite",
            null
        );

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ResendInvitation_ShouldReturnUnauthorized_WhenNotLoggedIn()
    {
        // Arrange
        var client = Factory.CreateClient();

        // Act
        var response = await client.PostAsync(
            $"/api/settings/team/{Guid.NewGuid()}/resend-invite",
            null
        );

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // ───────────────────────── Cancel Invitation ─────────────────────────

    [Fact]
    public async Task CancelInvitation_ShouldSucceed_WhenPendingUser()
    {
        // Arrange — seed a pending user
        var accessToken = await GetAccessTokenAsync();
        var client = CreateAuthenticatedClient(accessToken);
        var pendingUserId = Guid.NewGuid();

        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<FairWorklyDbContext>();
            var secretHasher = scope.ServiceProvider.GetRequiredService<ISecretHasher>();
            var adminUser = await db.Users.FirstAsync(u => u.Email == "test@example.com");

            var user = new User
            {
                Id = pendingUserId,
                Email = $"cancel_{Guid.NewGuid()}@example.com",
                FirstName = "Cancel",
                LastName = "Test",
                Role = UserRole.Manager,
                OrganizationId = adminUser.OrganizationId,
                IsActive = false,
                InvitationStatus = InvitationStatus.Pending,
                InvitationToken = secretHasher.Hash("cancel-token"),
                InvitationTokenExpiry = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();
        }

        // Act
        var response = await client.DeleteAsync($"/api/settings/team/{pendingUserId}/invite");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify user is soft-deleted and invitation cleared
        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<FairWorklyDbContext>();
            var user = await db.Users.IgnoreQueryFilters().FirstAsync(u => u.Id == pendingUserId);

            Assert.True(user.IsDeleted);
            Assert.False(user.IsActive);
            Assert.Equal(InvitationStatus.None, user.InvitationStatus);
            Assert.Null(user.InvitationToken);
            Assert.Null(user.InvitationTokenExpiry);
        }
    }

    [Fact]
    public async Task CancelInvitation_ShouldFail_WhenUserAlreadyAccepted()
    {
        // Arrange — seed a user who already accepted
        var accessToken = await GetAccessTokenAsync();
        var client = CreateAuthenticatedClient(accessToken);
        var acceptedUserId = Guid.NewGuid();

        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<FairWorklyDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
            var adminUser = await db.Users.FirstAsync(u => u.Email == "test@example.com");

            var user = new User
            {
                Id = acceptedUserId,
                Email = $"cancel_accepted_{Guid.NewGuid()}@example.com",
                FirstName = "Already",
                LastName = "Accepted",
                Role = UserRole.Manager,
                OrganizationId = adminUser.OrganizationId,
                IsActive = true,
                InvitationStatus = InvitationStatus.Accepted,
                PasswordHash = passwordHasher.Hash("ExistingPassword123"),
                CreatedAt = DateTime.UtcNow,
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();
        }

        // Act
        var response = await client.DeleteAsync($"/api/settings/team/{acceptedUserId}/invite");

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task CancelInvitation_ShouldFail_WhenUserNotFound()
    {
        // Arrange
        var accessToken = await GetAccessTokenAsync();
        var client = CreateAuthenticatedClient(accessToken);
        var nonExistentUserId = Guid.NewGuid();

        // Act
        var response = await client.DeleteAsync($"/api/settings/team/{nonExistentUserId}/invite");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CancelInvitation_ShouldReturnUnauthorized_WhenNotLoggedIn()
    {
        // Arrange
        var client = Factory.CreateClient();

        // Act
        var response = await client.DeleteAsync($"/api/settings/team/{Guid.NewGuid()}/invite");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CancelInvitation_ShouldFail_WhenUserBelongsToAnotherOrganization()
    {
        // Arrange — create a second org and a pending user in it
        var accessToken = await GetAccessTokenAsync();
        var client = CreateAuthenticatedClient(accessToken);
        var otherOrgUserId = Guid.NewGuid();

        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<FairWorklyDbContext>();
            var secretHasher = scope.ServiceProvider.GetRequiredService<ISecretHasher>();

            var otherOrg = new Organization
            {
                Id = Guid.NewGuid(),
                CompanyName = "Other Corp Pty Ltd",
                ABN = Guid.NewGuid().ToString("N")[..11],
                IndustryType = "Hospitality",
                AddressLine1 = "456 Other St",
                Suburb = "Sydney",
                State = AustralianState.NSW,
                Postcode = "2000",
                ContactEmail = "contact@othercorp.com.au",
                SubscriptionTier = SubscriptionTier.Tier1,
                SubscriptionStartDate = DateTime.UtcNow,
                IsSubscriptionActive = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false,
            };
            db.Organizations.Add(otherOrg);

            var pendingUserInOtherOrg = new User
            {
                Id = otherOrgUserId,
                Email = $"other_org_{Guid.NewGuid()}@example.com",
                FirstName = "Other",
                LastName = "OrgUser",
                Role = UserRole.Manager,
                OrganizationId = otherOrg.Id,
                IsActive = false,
                InvitationStatus = InvitationStatus.Pending,
                InvitationToken = secretHasher.Hash(Guid.NewGuid().ToString()),
                InvitationTokenExpiry = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
            };
            db.Users.Add(pendingUserInOtherOrg);
            await db.SaveChangesAsync();
        }

        // Act — admin from org A tries to cancel user from org B
        var response = await client.DeleteAsync($"/api/settings/team/{otherOrgUserId}/invite");

        // Assert — returns 404 (org mismatch is intentionally opaque)
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ───────────────────────── End-to-End Flow ─────────────────────────

    [Fact]
    public async Task InvitationFlow_EndToEnd_InviteThenAccept()
    {
        // 1. Admin invites a new member
        var accessToken = await GetAccessTokenAsync();
        var adminClient = CreateAuthenticatedClient(accessToken);
        var uniqueEmail = $"e2e_{Guid.NewGuid()}@example.com";

        var inviteRequest = new
        {
            Email = uniqueEmail,
            FirstName = "EndToEnd",
            LastName = "User",
            Role = "Admin",
        };

        var inviteResponse = await adminClient.PostAsJsonAsync(
            "/api/settings/team/invite",
            inviteRequest
        );
        Assert.Equal(HttpStatusCode.Created, inviteResponse.StatusCode);

        var inviteJson = JsonDocument
            .Parse(await inviteResponse.Content.ReadAsStringAsync())
            .RootElement;
        var inviteLink = inviteJson.GetProperty("data").GetProperty("inviteLink").GetString()!;

        // Extract plain token from invite link
        var plainToken = inviteLink.Split("token=")[1];

        // 2. Invited user accepts the invitation
        var anonClient = Factory.CreateClient();
        var acceptRequest = new { Token = plainToken, Password = "SecureNewPassword123" };

        var acceptResponse = await anonClient.PostAsJsonAsync("/api/invite/accept", acceptRequest);
        acceptResponse.EnsureSuccessStatusCode();

        var acceptJson = JsonDocument
            .Parse(await acceptResponse.Content.ReadAsStringAsync())
            .RootElement;
        Assert.Equal(uniqueEmail, acceptJson.GetProperty("data").GetProperty("email").GetString());

        // 3. Verify the new user can log in
        var loginResponse = await Client.PostAsJsonAsync(
            "/api/auth/login",
            new { Email = uniqueEmail, Password = "SecureNewPassword123" }
        );

        loginResponse.EnsureSuccessStatusCode();

        // 4. Verify the new user appears in team list
        var teamResponse = await adminClient.GetAsync("/api/settings/team");
        var teamJson = JsonDocument
            .Parse(await teamResponse.Content.ReadAsStringAsync())
            .RootElement;
        var members = teamJson.GetProperty("data");

        Assert.Contains(
            members.EnumerateArray(),
            m =>
                m.GetProperty("email").GetString() == uniqueEmail
                && m.GetProperty("role").GetString() == "Admin"
                && m.GetProperty("isActive").GetBoolean() == true
        );
    }
}
