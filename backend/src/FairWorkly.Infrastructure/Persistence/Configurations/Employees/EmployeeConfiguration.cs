using FairWorkly.Domain.Employees.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FairWorkly.Infrastructure.Persistence.Configurations.Employees;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("employees");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.LastName).IsRequired().HasMaxLength(100);

        builder.Property(e => e.BaseHourlyRate).HasColumnType("numeric(10,2)");

        // 这里的 TenantId 实际上应该有外键约束
        // 但为了 MVP 快速跑通 Employee 表，我们先暂且把它当普通字段，后续再加 Tenant 实体
        builder.Property(e => e.TenantId).IsRequired();
    }
}