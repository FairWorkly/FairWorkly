using FairWorkly.Domain.Payroll.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FairWorkly.Infrastructure.Persistence.Configurations.Payroll;

public class PayrollIssueConfiguration : IEntityTypeConfiguration<PayrollIssue>
{
    public void Configure(EntityTypeBuilder<PayrollIssue> builder)
    {
        // Explicit primary key
        builder.HasKey(i => i.Id);

        // Decimal precision settings to prevent truncation
        // SQL Server default may be (18,2); explicit is safer
        builder.Property(i => i.ExpectedValue).HasPrecision(18, 2);
        builder.Property(i => i.ActualValue).HasPrecision(18, 2);
        builder.Property(i => i.AffectedUnits).HasPrecision(18, 2);
        builder.Property(i => i.ImpactAmount).HasPrecision(18, 2);

        // Enum to string conversion for SQL readability
        builder.Property(i => i.Severity).HasConversion<string>();
        builder.Property(i => i.CategoryType).HasConversion<string>();

        // Relationship configuration

        // Link to Payslip (primary path)
        // If payslip is deleted, the issue should be deleted (cascade)
        builder
            .HasOne(i => i.Payslip)
            .WithMany(p => p.Issues)
            .HasForeignKey(i => i.PayslipId)
            .OnDelete(DeleteBehavior.Cascade);

        // Link to PayrollValidation (secondary path)
        // Set to NoAction to avoid SQL Server "Multiple Cascade Paths" error.
        // Deleting a validation deletes payslips, which cascade to issues.
        builder
            .HasOne(i => i.PayrollValidation)
            .WithMany(pv => pv.Issues) // Ensure PayrollValidation has ICollection<PayrollIssue> Issues
            .HasForeignKey(i => i.PayrollValidationId)
            .OnDelete(DeleteBehavior.NoAction);

        // Link to Employee
        // Prevent deleting issues when employee is removed; restrict deletion
        builder
            .HasOne(i => i.Employee)
            .WithMany()
            .HasForeignKey(i => i.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Link to Organization
        builder
            .HasOne(i => i.Organization)
            .WithMany()
            .HasForeignKey(i => i.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Link to User (ResolvedBy)
        builder
            .HasOne(i => i.ResolvedByUser)
            .WithMany()
            .HasForeignKey(i => i.ResolvedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
