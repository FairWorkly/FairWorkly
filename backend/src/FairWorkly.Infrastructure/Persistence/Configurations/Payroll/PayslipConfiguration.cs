using FairWorkly.Domain.Payroll.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FairWorkly.Infrastructure.Persistence.Configurations.Payroll;

public class PayslipConfiguration : IEntityTypeConfiguration<Payslip>
{
    public void Configure(EntityTypeBuilder<Payslip> builder)
    {
        // Set decimal precision (18,2) for financial fields
        builder.Property(p => p.HourlyRate).HasPrecision(18, 2);
        builder.Property(p => p.OrdinaryHours).HasPrecision(18, 2);
        builder.Property(p => p.OrdinaryPay).HasPrecision(18, 2);
        builder.Property(p => p.SaturdayHours).HasPrecision(18, 2);
        builder.Property(p => p.SaturdayPay).HasPrecision(18, 2);
        builder.Property(p => p.SundayHours).HasPrecision(18, 2);
        builder.Property(p => p.SundayPay).HasPrecision(18, 2);
        builder.Property(p => p.PublicHolidayHours).HasPrecision(18, 2);
        builder.Property(p => p.PublicHolidayPay).HasPrecision(18, 2);
        builder.Property(p => p.OvertimeHours).HasPrecision(18, 2);
        builder.Property(p => p.OvertimePay).HasPrecision(18, 2);
        builder.Property(p => p.Allowances).HasPrecision(18, 2);
        builder.Property(p => p.CasualLoadingPay).HasPrecision(18, 2);
        builder.Property(p => p.GrossPay).HasPrecision(18, 2);
        builder.Property(p => p.Tax).HasPrecision(18, 2);
        builder.Property(p => p.Superannuation).HasPrecision(18, 2);
        builder.Property(p => p.OtherDeductions).HasPrecision(18, 2);
        builder.Property(p => p.NetPay).HasPrecision(18, 2);

        // Explicit relationship configuration to be safe
        builder
            .HasOne(p => p.Employee)
            .WithMany(e => e.Payslips)
            .HasForeignKey(p => p.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent employee deletion from removing payslips

        builder
            .HasMany(p => p.Issues)
            .WithOne(i => i.Payslip)
            .HasForeignKey(i => i.PayslipId)
            .OnDelete(DeleteBehavior.Cascade); // Delete related issues when a payslip is deleted
    }
}
