using FairWorkly.Domain.Employees.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FairWorkly.Infrastructure.Persistence.Configurations.Employees;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasKey(e => e.Id);

        // Enum to string
        builder.Property(e => e.EmploymentType).HasConversion<string>().HasMaxLength(50);

        builder.Property(e => e.ApplicableAward).HasConversion<string>().HasMaxLength(100);
    }
}
