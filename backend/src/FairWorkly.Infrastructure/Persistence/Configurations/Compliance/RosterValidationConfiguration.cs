using FairWorkly.Domain.Compliance.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FairWorkly.Infrastructure.Persistence.Configurations.Compliance;

/// <summary>
/// EF Core configuration for RosterValidation entity
/// RosterValidation is the dependent side of the one-to-one relationship with Roster
/// </summary>
public class RosterValidationConfiguration : IEntityTypeConfiguration<RosterValidation>
{
    public void Configure(EntityTypeBuilder<RosterValidation> builder)
    {
        builder.HasKey(rv => rv.Id);

        // Configure AuditableEntity navigation properties
        builder.HasOne(rv => rv.CreatedByUser)
            .WithMany()
            .HasForeignKey(rv => rv.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(rv => rv.UpdatedByUser)
            .WithMany()
            .HasForeignKey(rv => rv.UpdatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // RosterValidation -> Organization
        builder.HasOne(rv => rv.Organization)
            .WithMany()
            .HasForeignKey(rv => rv.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        // RosterValidation -> Roster (One-to-One)
        // Configured in RosterConfiguration as the principal side

        // RosterValidation -> RosterIssues (One-to-Many)
        builder.HasMany(rv => rv.Issues)
            .WithOne(i => i.RosterValidation)
            .HasForeignKey(i => i.RosterValidationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(rv => rv.RosterId).IsUnique();
        builder.HasIndex(rv => new { rv.OrganizationId, rv.Status });

        // Ignore computed properties
        builder.Ignore(rv => rv.PassRate);
        builder.Ignore(rv => rv.DurationSeconds);

        // Property configurations
        builder.Property(rv => rv.Notes).HasMaxLength(1000);
    }
}
