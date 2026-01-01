using FairWorkly.Domain.Auth.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FairWorkly.Infrastructure.Persistence.Configurations.Auth;

/// <summary>
/// EF Core configuration for OrganizationAward entity
/// Many-to-many relationship between Organization and Awards
/// </summary>
public class OrganizationAwardConfiguration : IEntityTypeConfiguration<OrganizationAward>
{
    public void Configure(EntityTypeBuilder<OrganizationAward> builder)
    {
        builder.HasKey(oa => oa.Id);

        // OrganizationAward -> Organization
        builder.HasOne(oa => oa.Organization)
            .WithMany()
            .HasForeignKey(oa => oa.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        // Unique constraint: One organization can have each award type only once
        builder.HasIndex(oa => new { oa.OrganizationId, oa.AwardType }).IsUnique();
        // Quick lookup for primary awards
        builder.HasIndex(oa => new { oa.OrganizationId, oa.IsPrimary });
    }
}
