using FairWorkly.Domain.Payroll.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FairWorkly.Infrastructure.Persistence.Configurations.Payroll;

public class PayrollValidationConfiguration : IEntityTypeConfiguration<PayrollValidation>
{
    public void Configure(EntityTypeBuilder<PayrollValidation> builder)
    {
        // Explicit primary key for clarity
        builder.HasKey(pv => pv.Id);

        // Configure one-to-many (Validation -> Payslips)
        // Deleting a validation deletes related payslips
        builder
            .HasMany(pv => pv.Payslips)
            .WithOne(p => p.PayrollValidation)
            .HasForeignKey(p => p.PayrollValidationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure one-to-many (Validation -> Issues)
        // These issues are global ones attached to the validation, not payslip-specific
        // Use NoAction to avoid cascade path conflicts with Payslip deletion
        builder
            .HasMany(pv => pv.Issues)
            .WithOne(i => i.PayrollValidation)
            .HasForeignKey(i => i.PayrollValidationId)
            .OnDelete(DeleteBehavior.NoAction);

        // Ignore computed properties (C# only, not stored in DB)
        builder.Ignore(pv => pv.DurationSeconds);
    }
}
