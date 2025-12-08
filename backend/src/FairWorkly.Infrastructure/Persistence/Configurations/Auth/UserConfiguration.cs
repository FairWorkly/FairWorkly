using FairWorkly.Domain.Auth.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FairWorkly.Infrastructure.Persistence.Configurations.Auth;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Primary Key
        builder.HasKey(u => u.Id);

        // Core fields
        builder.Property(u => u.Email).IsRequired().HasMaxLength(255);

        // Ensure email is unique
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.FirstName).HasMaxLength(100);
        builder.Property(u => u.LastName).HasMaxLength(100);

        // Password hash (BCrypt)
        builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(500);

        // user.role is an enum and needs to be stored as a string
        builder.Property(u => u.Role).HasConversion<string>().HasMaxLength(50);

        // Relationship configuration: One company has multiple users
        builder
            .HasOne(u => u.Organization)
            .WithMany(o => o.Users)
            .HasForeignKey(u => u.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade); // If company is deleted, employees are also deleted
    }
}
