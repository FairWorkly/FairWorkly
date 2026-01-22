using FairWorkly.Domain.Auth.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FairWorkly.Infrastructure.Persistence.Configurations.Auth;

/// <summary>
/// EF Core configuration for Organization entity
/// Configures relationships inherited from AuditableEntity
/// </summary>
public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        // Configure AuditableEntity navigation properties
        // These must be explicitly configured because EF Core cannot infer
        // the relationship when multiple entities reference the same User entity

        builder.HasOne(o => o.CreatedByUser)
            .WithMany()
            .HasForeignKey(o => o.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.UpdatedByUser)
            .WithMany()
            .HasForeignKey(o => o.UpdatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Organization-specific relationships
        builder.HasMany(o => o.Users)
            .WithOne(u => u.Organization)
            .HasForeignKey(u => u.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.Employees)
            .WithOne(e => e.Organization)
            .HasForeignKey(e => e.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(o => o.ABN).IsUnique();
        builder.HasIndex(o => o.ContactEmail);

        // Property configurations
        builder.Property(o => o.CompanyName).HasMaxLength(200).IsRequired();
        builder.Property(o => o.ABN).HasMaxLength(11).IsRequired();
        builder.Property(o => o.IndustryType).HasMaxLength(100).IsRequired();
        builder.Property(o => o.AddressLine1).HasMaxLength(200).IsRequired();
        builder.Property(o => o.AddressLine2).HasMaxLength(200);
        builder.Property(o => o.Suburb).HasMaxLength(100).IsRequired();
        builder.Property(o => o.Postcode).HasMaxLength(4).IsRequired();
        builder.Property(o => o.ContactEmail).HasMaxLength(255).IsRequired();
        builder.Property(o => o.PhoneNumber).HasMaxLength(20);
        builder.Property(o => o.LogoUrl).HasMaxLength(500);
    }
}
