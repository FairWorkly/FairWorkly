using FairWorkly.Domain.Compliance.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FairWorkly.Infrastructure.Persistence.Configurations.Compliance;

/// <summary>
/// EF Core configuration for RosterIssue entity
/// Note: RosterIssue inherits from BaseEntity (not AuditableEntity)
/// </summary>
public class RosterIssueConfiguration : IEntityTypeConfiguration<RosterIssue>
{
    public void Configure(EntityTypeBuilder<RosterIssue> builder)
    {
        builder.HasKey(ri => ri.Id);

        // RosterIssue -> Organization
        builder.HasOne(ri => ri.Organization)
            .WithMany()
            .HasForeignKey(ri => ri.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        // RosterIssue -> RosterValidation (configured in RosterValidationConfiguration)
        builder.HasOne(ri => ri.RosterValidation)
            .WithMany(rv => rv.Issues)
            .HasForeignKey(ri => ri.RosterValidationId)
            .OnDelete(DeleteBehavior.Cascade);

        // RosterIssue -> Roster (configured in RosterConfiguration)
        builder.HasOne(ri => ri.Roster)
            .WithMany(r => r.Issues)
            .HasForeignKey(ri => ri.RosterId)
            .OnDelete(DeleteBehavior.NoAction);

        // RosterIssue -> Shift (optional, configured in ShiftConfiguration)
        builder.HasOne(ri => ri.Shift)
            .WithMany(s => s.Issues)
            .HasForeignKey(ri => ri.ShiftId)
            .OnDelete(DeleteBehavior.NoAction);

        // RosterIssue -> Employee
        builder.HasOne(ri => ri.Employee)
            .WithMany()
            .HasForeignKey(ri => ri.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // RosterIssue -> ResolvedByUser (optional)
        builder.HasOne(ri => ri.ResolvedByUser)
            .WithMany()
            .HasForeignKey(ri => ri.ResolvedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // RosterIssue -> WaivedByUser (optional)
        builder.HasOne(ri => ri.WaivedByUser)
            .WithMany()
            .HasForeignKey(ri => ri.WaivedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(ri => new { ri.RosterValidationId, ri.Severity });
        builder.HasIndex(ri => new { ri.RosterId, ri.EmployeeId });
        builder.HasIndex(ri => ri.CheckType);

        // Ignore computed properties
        builder.Ignore(ri => ri.Variance);

        // Property configurations
        builder.Property(ri => ri.CheckType).HasMaxLength(100).IsRequired();
        builder.Property(ri => ri.Description).HasMaxLength(500).IsRequired();
        builder.Property(ri => ri.DetailedExplanation).HasMaxLength(2000);
        builder.Property(ri => ri.Recommendation).HasMaxLength(1000);
        builder.Property(ri => ri.AffectedDates).HasMaxLength(500);
        builder.Property(ri => ri.ResolutionNotes).HasMaxLength(1000);
        builder.Property(ri => ri.WaiverReason).HasMaxLength(1000);
        builder.Property(ri => ri.ExpectedValue).HasPrecision(18, 4);
        builder.Property(ri => ri.ActualValue).HasPrecision(18, 4);
    }
}
