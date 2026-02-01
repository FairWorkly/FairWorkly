using FairWorkly.Domain.Employees.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FairWorkly.Infrastructure.Persistence.Configurations.Employees;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        // Basic field constraints
        builder.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.LastName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Email).HasMaxLength(255);
        builder.Property(e => e.EmployeeNumber).HasMaxLength(50);

        // Important: create indexes to speed up CSV lookups
        // Composite index: EmployeeNumber must be unique within the same Organization
        builder
            .HasIndex(e => new { e.OrganizationId, e.EmployeeNumber })
            .IsUnique()
            .HasFilter("employee_number IS NOT NULL");

        // Email is unique when provided (null emails allowed for imported employees)
        builder
            .HasIndex(e => new { e.OrganizationId, e.Email })
            .IsUnique()
            .HasFilter("email IS NOT NULL");

        // Relationship configuration
        builder
            .HasOne(e => e.Organization)
            .WithMany(o => o.Employees)
            .HasForeignKey(e => e.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        // audit relationship configurations
        builder
            .HasOne(e => e.CreatedByUser)
            .WithMany()
            .HasForeignKey(e => e.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(e => e.UpdatedByUser)
            .WithMany()
            .HasForeignKey(e => e.UpdatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Employee -> Payslips already configured in PayslipConfiguration
        // Optional here as a safeguard; EF Core allows both sides if consistent
        builder
            .HasMany(e => e.Payslips)
            .WithOne(p => p.Employee)
            .HasForeignKey(p => p.EmployeeId);
    }
}
