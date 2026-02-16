using System.Net;
using System.Net.Http.Json;
using FairWorkly.Application.Settings.Features.GetTeamMembers;
using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Auth.Enums;
using FairWorkly.Infrastructure.Persistence;
using FairWorkly.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FairWorkly.IntegrationTests.Settings;

public class TeamManagementTests : AuthTestsBase
{
    public TeamManagementTests(CustomWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task GetTeamMembers_ShouldReturnMembers_WhenAdmin()
    {
        // Arrange
        var accessToken = await GetAccessTokenAsync();
        var client = CreateAuthenticatedClient(accessToken);

        // Act
        var response = await client.GetAsync("/api/settings/team");

        // Assert
        response.EnsureSuccessStatusCode();
        var members = await response.Content.ReadFromJsonAsync<List<TeamMemberDto>>();

        Assert.NotNull(members);
        Assert.NotEmpty(members);
        Assert.Contains(members, m => m.Email == "test@example.com");
    }

    [Fact]
    public async Task UpdateTeamMember_ShouldUpdateRole_WhenAdmin()
    {
        // Arrange
        var accessToken = await GetAccessTokenAsync();
        var client = CreateAuthenticatedClient(accessToken);

        // Create a user to update
        var targetUserId = Guid.NewGuid();
        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<FairWorklyDbContext>();
            var adminUser = await db.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");
            var orgId = adminUser!.OrganizationId;

            var userToUpdate = new User
            {
                Id = targetUserId,
                Email = $"update_test_{Guid.NewGuid()}@example.com",
                FirstName = "Update",
                LastName = "Test",
                Role = UserRole.Manager, // Start as Manager
                IsActive = true,
                OrganizationId = orgId,
                PasswordHash = "hash",
                CreatedAt = DateTime.UtcNow,
            };
            db.Users.Add(userToUpdate);
            await db.SaveChangesAsync();
        }

        var request = new { Role = UserRole.Admin.ToString(), IsActive = true };

        // Act
        var response = await client.PatchAsJsonAsync($"/api/settings/team/{targetUserId}", request);

        // Assert
        response.EnsureSuccessStatusCode();

        // Verify in DB
        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<FairWorklyDbContext>();
            var updatedUser = await db.Users.FindAsync(targetUserId);
            Assert.Equal(UserRole.Admin, updatedUser!.Role);
        }
    }

    [Fact]
    public async Task UpdateTeamMember_ShouldFail_WhenDemotingSelf()
    {
        // Arrange
        var accessToken = await GetAccessTokenAsync(); // Logs in as test@example.com (Admin)
        var client = CreateAuthenticatedClient(accessToken);

        // Get current user ID
        Guid currentUserId;
        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<FairWorklyDbContext>();
            var user = await db.Users.FirstAsync(u => u.Email == "test@example.com");
            currentUserId = user.Id;
        }

        var request = new
        {
            Role = UserRole.Manager.ToString(), // Demotion from Admin to Manager
            IsActive = true,
        };

        // Act
        var response = await client.PatchAsJsonAsync(
            $"/api/settings/team/{currentUserId}",
            request
        );

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode); // Or BadRequest, depends on implementation. Controller says 403 (Forbidden) for result.Type == Forbidden
    }

    [Fact]
    public async Task UpdateTeamMember_ShouldFail_WhenDeactivatingSelf()
    {
        // Arrange
        var accessToken = await GetAccessTokenAsync(); // Logs in as test@example.com (Admin)
        var client = CreateAuthenticatedClient(accessToken);

        // Get current user ID
        Guid currentUserId;
        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<FairWorklyDbContext>();
            var user = await db.Users.FirstAsync(u => u.Email == "test@example.com");
            currentUserId = user.Id;
        }

        var request = new
        {
            Role = UserRole.Admin.ToString(),
            IsActive = false, // Deactivation
        };

        // Act
        var response = await client.PatchAsJsonAsync(
            $"/api/settings/team/{currentUserId}",
            request
        );

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetTeamMembers_ShouldReturnUnauthorized_WhenNotLoggedIn()
    {
        // Arrange
        var client = Factory.CreateClient(); // No auth header

        // Act
        var response = await client.GetAsync("/api/settings/team");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetTeamMembers_ShouldReturnMembers_WhenManager()
    {
        // Arrange
        // 1. Create a Manager user
        var managerEmail = $"manager_{Guid.NewGuid()}@example.com";
        var password = "TestPassword123";

        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<FairWorklyDbContext>();
            var hasher =
                scope.ServiceProvider.GetRequiredService<FairWorkly.Application.Common.Interfaces.IPasswordHasher>();

            // Get valid org
            var adminUser = await db.Users.FirstAsync(u => u.Email == "test@example.com");

            var manager = new User
            {
                Id = Guid.NewGuid(),
                Email = managerEmail,
                FirstName = "Manager",
                LastName = "User",
                Role = UserRole.Manager,
                IsActive = true,
                OrganizationId = adminUser.OrganizationId,
                PasswordHash = hasher.Hash(password),
                CreatedAt = DateTime.UtcNow,
            };
            db.Users.Add(manager);
            await db.SaveChangesAsync();
        }

        // 2. Login as Manager
        var accessToken = await GetAccessTokenAsync(managerEmail, password);
        var client = CreateAuthenticatedClient(accessToken);

        // Act
        var response = await client.GetAsync("/api/settings/team");

        // Assert
        response.EnsureSuccessStatusCode();
        var members = await response.Content.ReadFromJsonAsync<List<TeamMemberDto>>();
        Assert.NotNull(members);
        Assert.NotEmpty(members);
    }

    [Fact]
    public async Task UpdateTeamMember_ShouldReturnForbidden_WhenManager()
    {
        // Arrange
        // 1. Create a Manager user
        var managerEmail = $"manager_upd_{Guid.NewGuid()}@example.com";
        var password = "TestPassword123";

        Guid targetUserId;

        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<FairWorklyDbContext>();
            var hasher =
                scope.ServiceProvider.GetRequiredService<FairWorkly.Application.Common.Interfaces.IPasswordHasher>();

            // Get valid org and a target user
            var adminUser = await db.Users.FirstAsync(u => u.Email == "test@example.com");
            targetUserId = adminUser.Id; // Try to update the admin

            var manager = new User
            {
                Id = Guid.NewGuid(),
                Email = managerEmail,
                FirstName = "Manager",
                LastName = "User",
                Role = UserRole.Manager,
                IsActive = true,
                OrganizationId = adminUser.OrganizationId,
                PasswordHash = hasher.Hash(password),
                CreatedAt = DateTime.UtcNow,
            };
            db.Users.Add(manager);
            await db.SaveChangesAsync();
        }

        // 2. Login as Manager
        var accessToken = await GetAccessTokenAsync(managerEmail, password);
        var client = CreateAuthenticatedClient(accessToken);

        var request = new
        {
            Role = UserRole.Admin.ToString(), // Try to promote self or change other
            IsActive = true,
        };

        // Act
        var response = await client.PatchAsJsonAsync($"/api/settings/team/{targetUserId}", request);

        // Assert
        // Should be 403 because of [Authorize(Policy = "RequireAdmin")]
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
