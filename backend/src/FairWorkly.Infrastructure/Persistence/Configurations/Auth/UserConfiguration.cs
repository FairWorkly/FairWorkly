using FairWorkly.Domain.Auth.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FairWorkly.Infrastructure.Persistence.Configurations.Auth;

/// <summary>
/// EF Core configuration for User entity
/// Configures relationships inherited from AuditableEntity
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Configure AuditableEntity navigation properties
        // These must be explicitly configured because EF Core cannot infer
        // the relationship when multiple entities reference the same User entity

        builder.HasOne(u => u.CreatedByUser)
            .WithMany()
            .HasForeignKey(u => u.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.UpdatedByUser)
            .WithMany()
            .HasForeignKey(u => u.UpdatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // User -> Organization relationship is configured in OrganizationConfiguration

        // User -> Employee (optional link for Employee role users)
        builder.HasOne(u => u.Employee)
            .WithMany()
            .HasForeignKey(u => u.EmployeeId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(u => new { u.OrganizationId, u.Email }).IsUnique();
        builder.HasIndex(u => u.GoogleId).IsUnique().HasFilter("google_id IS NOT NULL");

        // Property configurations
        builder.Property(u => u.Email).HasMaxLength(255).IsRequired();
        builder.Property(u => u.PasswordHash).HasMaxLength(500).IsRequired();
        builder.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(u => u.LastName).HasMaxLength(100).IsRequired();
        builder.Property(u => u.PhoneNumber).HasMaxLength(20);
        builder.Property(u => u.RefreshToken).HasMaxLength(500);
        builder.Property(u => u.PasswordResetToken).HasMaxLength(500);
        builder.Property(u => u.GoogleId).HasMaxLength(100);
    }
}
