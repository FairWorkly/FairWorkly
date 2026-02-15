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
public abstract class IntegrationTestBase
    : IClassFixture<CustomWebApplicationFactory>,
        IAsyncLifetime
{
    private static readonly SemaphoreSlim _seedSemaphore = new(1, 1);
    private static bool _seeded = false;

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
        Client.Dispose();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Seed test data for non-Auth integration tests (idempotent + thread-safe).
    /// Uses SemaphoreSlim to prevent race conditions when xUnit runs test classes in parallel.
    /// Creates a separate Organization + User from Auth tests to avoid data conflicts.
    /// </summary>
    private async Task SeedTestDataAsync()
    {
        // Fast path: already seeded in this test run, just load IDs
        if (_seeded)
        {
            await LoadTestIdsAsync();
            return;
        }

        await _seedSemaphore.WaitAsync();
        try
        {
            // Double-check after acquiring the lock
            if (_seeded)
            {
                await LoadTestIdsAsync();
                return;
            }

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
                TestUserId =
                    existingUser?.Id
                    ?? throw new InvalidOperationException(
                        "Test user not found for Integration Test Company"
                    );
                _seeded = true;
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
            _seeded = true;
        }
        finally
        {
            _seedSemaphore.Release();
        }
    }

    /// <summary>
    /// Load test data IDs from DB (fast path when seed already completed).
    /// </summary>
    private async Task LoadTestIdsAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FairWorklyDbContext>();
        var org = await db.Set<Organization>()
            .FirstOrDefaultAsync(o => o.CompanyName == "Integration Test Company");
        TestOrganizationId = org!.Id;
        var user = await db.Set<User>().FirstOrDefaultAsync(u => u.OrganizationId == org.Id);
        TestUserId =
            user?.Id
            ?? throw new InvalidOperationException(
                "Test user not found for Integration Test Company"
            );
    }

    /// <summary>
    /// Get an access token by logging in with the test user.
    /// </summary>
    protected async Task<string> GetAccessTokenAsync()
    {
        var response = await Client.PostAsJsonAsync(
            "/api/auth/login",
            new { email = "testuser@integrationtest.com", password = "TestPassword123" }
        );
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("data").GetProperty("accessToken").GetString()!;
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
