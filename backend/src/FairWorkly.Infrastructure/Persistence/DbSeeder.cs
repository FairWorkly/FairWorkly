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
        var awards = context.Set<OrganizationAward>();

        // Backfill: runs on every startup so existing local DBs get the demo award
        // without requiring a full data reset.
        var demoOrg = await organizations.FirstOrDefaultAsync(o =>
            o.ContactEmail == "contact@fairworkly.com.au" && !o.IsDeleted
        );

        if (
            demoOrg != null
            && !await awards.AnyAsync(oa =>
                oa.OrganizationId == demoOrg.Id && oa.IsPrimary && !oa.IsDeleted
            )
        )
        {
            awards.Add(
                new OrganizationAward
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = demoOrg.Id,
                    AwardType = AwardType.GeneralRetailIndustryAward2020,
                    IsPrimary = true,
                    EmployeeCount = 0,
                    AddedAt = DateTime.UtcNow,
                    CreatedAt = DateTimeOffset.UtcNow,
                    IsDeleted = false,
                }
            );
            await context.SaveChangesAsync();
        }

        // Full seed only runs on a brand-new database.
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

        var managerUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "manager@fairworkly.com.au",
            FirstName = "Manager",
            LastName = "User",
            Role = UserRole.Manager,
            IsActive = true,
            OrganizationId = organizationId,
            PasswordHash = passwordHasher.Hash("fairworkly123"),
            CreatedAt = now,
            IsDeleted = false,
        };

        var demoOrgAward = new OrganizationAward
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            AwardType = AwardType.GeneralRetailIndustryAward2020,
            IsPrimary = true,
            EmployeeCount = 0,
            AddedAt = now.UtcDateTime,
            CreatedAt = now,
            IsDeleted = false,
        };

        organizations.Add(demoOrg);
        users.Add(adminUser);
        users.Add(managerUser);
        context.Set<OrganizationAward>().Add(demoOrgAward);

        await context.SaveChangesAsync();
    }
}
