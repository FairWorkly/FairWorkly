using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Auth.Enums;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FairWorkly.IntegrationTests.Infrastructure;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    // The JWT secret used in tests - must match what's set in environment variables
    public const string TestJwtSecret = "integration-test-secret-key-at-least-32-characters-long";

    // Use a static database name to ensure consistency across all test classes
    private static readonly string DatabaseName = "IntegrationTestDb_" + Guid.NewGuid().ToString();
    private static bool _seeded = false;
    private static readonly object _seedLock = new();

    public CustomWebApplicationFactory()
    {
        // Set environment variables BEFORE the host is created
        // These override any values from appsettings.json
        Environment.SetEnvironmentVariable("JwtSettings__Secret", TestJwtSecret);
        Environment.SetEnvironmentVariable("JwtSettings__Issuer", "FairWorkly.Auth");
        Environment.SetEnvironmentVariable("JwtSettings__Audience", "FairWorkly.Web");
        Environment.SetEnvironmentVariable("JwtSettings__ExpiryMinutes", "15");
        Environment.SetEnvironmentVariable("JwtSettings__RefreshTokenExpiryDays", "7");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<FairWorklyDbContext>)
            );

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add InMemory database for testing - use same name for all tests
            services.AddDbContext<FairWorklyDbContext>(options =>
            {
                options.UseInMemoryDatabase(DatabaseName);
            });

            // Build service provider and seed test data (only once)
            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<FairWorklyDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

            db.Database.EnsureCreated();

            lock (_seedLock)
            {
                if (!_seeded)
                {
                    SeedTestData(db, passwordHasher);
                    _seeded = true;
                }
            }
        });

        builder.UseEnvironment("Development");
    }

    private static void SeedTestData(FairWorklyDbContext db, IPasswordHasher passwordHasher)
    {
        var now = DateTimeOffset.UtcNow;
        var organizationId = Guid.NewGuid();

        // Create test organization
        var testOrg = new Organization
        {
            Id = organizationId,
            CompanyName = "Test Corp Pty Ltd",
            ABN = "12345678901",
            IndustryType = "Retail",
            AddressLine1 = "123 Test St",
            Suburb = "Melbourne",
            State = AustralianState.VIC,
            Postcode = "3000",
            ContactEmail = "contact@test.com",
            SubscriptionTier = SubscriptionTier.Tier1,
            SubscriptionStartDate = now.UtcDateTime,
            IsSubscriptionActive = true,
            CurrentEmployeeCount = 0,
            CreatedAt = now,
            IsDeleted = false,
        };

        // Normal active user
        var normalUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            Role = UserRole.Admin,
            IsActive = true,
            OrganizationId = organizationId,
            PasswordHash = passwordHasher.Hash("TestPassword123"),
            CreatedAt = now,
            IsDeleted = false,
        };

        // Disabled user
        var disabledUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "disabled@example.com",
            FirstName = "Disabled",
            LastName = "User",
            Role = UserRole.Admin,
            IsActive = false,
            OrganizationId = organizationId,
            PasswordHash = passwordHasher.Hash("TestPassword123"),
            CreatedAt = now,
            IsDeleted = false,
        };

        // User to be deleted (for testing user not found scenario)
        var toDeleteUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "todelete@example.com",
            FirstName = "ToDelete",
            LastName = "User",
            Role = UserRole.Admin,
            IsActive = true,
            OrganizationId = organizationId,
            PasswordHash = passwordHasher.Hash("TestPassword123"),
            CreatedAt = now,
            IsDeleted = false,
        };

        db.Set<Organization>().Add(testOrg);
        db.Set<User>().AddRange(normalUser, disabledUser, toDeleteUser);
        db.SaveChanges();
    }
}
