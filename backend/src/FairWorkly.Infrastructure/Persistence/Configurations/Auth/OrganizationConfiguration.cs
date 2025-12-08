using FairWorkly.Domain.Auth.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FairWorkly.Infrastructure.Persistence.Configurations.Auth;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        // Primary key
        builder.HasKey(o => o.Id);

        // Organization name is required and max length 200
        builder.Property(o => o.Name).IsRequired().HasMaxLength(200);

        // ABN has max length 20
        builder.Property(o => o.ABN).HasMaxLength(20);

        builder.Property(o => o.CreatedAt).IsRequired();
    }
}
