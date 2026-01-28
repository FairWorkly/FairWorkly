using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Auth.Enums;
using FairWorkly.Domain.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FairWorkly.Infrastructure.Persistence;

/// <summary>
/// Database seeder for local development.
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FairWorklyDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        var users = context.Set<User>();
        var organizations = context.Set<Organization>();

        if (await users.AnyAsync())
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var organizationId = Guid.NewGuid();

        var demoOrg = new Organization
        {
            Id = organizationId,
            CompanyName = "Demo Corp Pty Ltd",
            ABN = "12345678901",
            IndustryType = "Retail",
            AddressLine1 = "123 Demo St",
            AddressLine2 = "Level 1",
            Suburb = "Melbourne",
            State = AustralianState.VIC,
            Postcode = "3000",
            ContactEmail = "contact@fairworkly.com.au",
            PhoneNumber = "0400000000",
            SubscriptionTier = SubscriptionTier.Tier1,
            SubscriptionStartDate = now.UtcDateTime,
            IsSubscriptionActive = true,
            CurrentEmployeeCount = 0,
            CreatedAt = now,
            IsDeleted = false,
        };

        var adminUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "admin@fairworkly.com.au",
            FirstName = "Admin",
            LastName = "User",
            Role = UserRole.Admin,
            IsActive = true,
            OrganizationId = organizationId,
            PasswordHash = passwordHasher.Hash("fairworkly123"),
            CreatedAt = now,
            IsDeleted = false,
        };

        organizations.Add(demoOrg);
        users.Add(adminUser);

        await context.SaveChangesAsync();
    }
}
