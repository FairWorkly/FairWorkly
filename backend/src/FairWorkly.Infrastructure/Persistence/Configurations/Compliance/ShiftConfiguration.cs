using FairWorkly.Domain.Compliance.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FairWorkly.Infrastructure.Persistence.Configurations.Compliance;

/// <summary>
/// EF Core configuration for Shift entity
/// Note: Shift inherits from BaseEntity (not AuditableEntity)
/// </summary>
public class ShiftConfiguration : IEntityTypeConfiguration<Shift>
{
    public void Configure(EntityTypeBuilder<Shift> builder)
    {
        builder.HasKey(s => s.Id);

        // Shift -> Organization
        builder.HasOne(s => s.Organization)
            .WithMany()
            .HasForeignKey(s => s.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Shift -> Roster (configured in RosterConfiguration as well)
        builder.HasOne(s => s.Roster)
            .WithMany(r => r.Shifts)
            .HasForeignKey(s => s.RosterId)
            .OnDelete(DeleteBehavior.Cascade);

        // Shift -> Employee
        builder.HasOne(s => s.Employee)
            .WithMany(e => e.Shifts)
            .HasForeignKey(s => s.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Shift -> RosterIssues (One-to-Many, optional relationship)
        builder.HasMany(s => s.Issues)
            .WithOne(i => i.Shift)
            .HasForeignKey(i => i.ShiftId)
            .OnDelete(DeleteBehavior.NoAction);

        // Indexes
        builder.HasIndex(s => new { s.RosterId, s.EmployeeId, s.Date });
        builder.HasIndex(s => new { s.OrganizationId, s.Date });
        builder.HasIndex(s => new { s.EmployeeId, s.Date });

        // Ignore computed properties
        builder.Ignore(s => s.Duration);
        builder.Ignore(s => s.StartDateTime);
        builder.Ignore(s => s.EndDateTime);
        builder.Ignore(s => s.NetHours);

        // Property configurations
        builder.Property(s => s.PublicHolidayName).HasMaxLength(100);
        builder.Property(s => s.Location).HasMaxLength(100);
        builder.Property(s => s.Notes).HasMaxLength(500);
    }
}
