using System.Net.Http.Json;
using System.Text.Json;
using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Auth.Enums;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FairWorkly.IntegrationTests.Infrastructure;

/// <summary>
/// Integration test base class for non-Auth tests (Payroll, Roster, etc.)
/// Provides: test data seeding, token acquisition, authenticated client creation.
/// </summary>
public abstract class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    protected readonly CustomWebApplicationFactory Factory;
    protected readonly HttpClient Client;
    protected Guid TestOrganizationId { get; private set; }
    protected Guid TestUserId { get; private set; }

    protected IntegrationTestBase(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await SeedTestDataAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Seed test data for non-Auth integration tests (idempotent).
    /// Creates a separate Organization + User from Auth tests to avoid data conflicts.
    /// </summary>
    private async Task SeedTestDataAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FairWorklyDbContext>();

        // Check if test data already exists (idempotent for real PostgreSQL)
        var existingOrg = await db.Set<Organization>()
            .FirstOrDefaultAsync(o => o.CompanyName == "Integration Test Company");

        if (existingOrg != null)
        {
            TestOrganizationId = existingOrg.Id;
            var existingUser = await db.Set<User>()
                .FirstOrDefaultAsync(u => u.OrganizationId == existingOrg.Id);
            TestUserId = existingUser?.Id ?? Guid.Empty;
            return;
        }

        // Create test Organization (unique ABN to avoid collision with DbSeeder/Factory)
        TestOrganizationId = Guid.NewGuid();
        var organization = new Organization
        {
            Id = TestOrganizationId,
            CompanyName = "Integration Test Company",
            ABN = "99999999902",
            IndustryType = "Retail",
            AddressLine1 = "123 Test Street",
            Suburb = "Melbourne",
            State = AustralianState.VIC,
            Postcode = "3000",
            ContactEmail = "test@integrationtest.com",
            SubscriptionTier = SubscriptionTier.Tier1,
            SubscriptionStartDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
            IsSubscriptionActive = true,
        };

        // Create test User
        TestUserId = Guid.NewGuid();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var user = new User
        {
            Id = TestUserId,
            Email = "testuser@integrationtest.com",
            FirstName = "Test",
            LastName = "User",
            Role = UserRole.Admin,
            IsActive = true,
            OrganizationId = TestOrganizationId,
            PasswordHash = passwordHasher.Hash("TestPassword123"),
            CreatedAt = DateTimeOffset.UtcNow,
            IsDeleted = false,
        };

        db.Set<Organization>().Add(organization);
        db.Set<User>().Add(user);
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Get an access token by logging in with the test user.
    /// </summary>
    protected async Task<string> GetAccessTokenAsync()
    {
        var response = await Client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "testuser@integrationtest.com",
            password = "TestPassword123"
        });
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("accessToken").GetString()!;
    }

    /// <summary>
    /// Create an HttpClient with a valid Bearer token.
    /// </summary>
    protected async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var token = await GetAccessTokenAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
