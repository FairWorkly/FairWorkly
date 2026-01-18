using FairWorkly.Domain.Compliance.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FairWorkly.Infrastructure.Persistence.Configurations.Compliance;

/// <summary>
/// EF Core configuration for Roster entity
/// Configures one-to-one relationship with RosterValidation
/// </summary>
public class RosterConfiguration : IEntityTypeConfiguration<Roster>
{
    public void Configure(EntityTypeBuilder<Roster> builder)
    {
        builder.HasKey(r => r.Id);

        // Configure AuditableEntity navigation properties
        builder.HasOne(r => r.CreatedByUser)
            .WithMany()
            .HasForeignKey(r => r.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.UpdatedByUser)
            .WithMany()
            .HasForeignKey(r => r.UpdatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Roster -> Organization
        builder.HasOne(r => r.Organization)
            .WithMany()
            .HasForeignKey(r => r.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Roster -> RosterValidation (One-to-One, optional)
        // Roster is the principal, RosterValidation is the dependent
        // RosterValidation has the required FK (RosterId)
        // This side only has an optional reference (RosterValidationId is nullable)
        builder.HasOne(r => r.RosterValidation)
            .WithOne(rv => rv.Roster)
            .HasForeignKey<RosterValidation>(rv => rv.RosterId)
            .OnDelete(DeleteBehavior.Cascade);

        // Roster -> Shifts (One-to-Many)
        builder.HasMany(r => r.Shifts)
            .WithOne(s => s.Roster)
            .HasForeignKey(s => s.RosterId)
            .OnDelete(DeleteBehavior.Cascade);

        // Roster -> RosterIssues (One-to-Many)
        builder.HasMany(r => r.Issues)
            .WithOne(i => i.Roster)
            .HasForeignKey(i => i.RosterId)
            .OnDelete(DeleteBehavior.NoAction);

        // Indexes
        builder.HasIndex(r => new { r.OrganizationId, r.WeekStartDate });
        builder.HasIndex(r => new { r.OrganizationId, r.Year, r.WeekNumber }).IsUnique();

        // Property configurations
        builder.Property(r => r.Notes).HasMaxLength(1000);
        builder.Property(r => r.TotalHours).HasPrecision(10, 2);
    }
}
