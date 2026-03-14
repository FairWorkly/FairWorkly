using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Auth.Enums;
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
        // Table name (snake_case + plural)
        builder.ToTable("users");

        // Configure AuditableEntity navigation properties
        // These must be explicitly configured because EF Core cannot infer
        // the relationship when multiple entities reference the same User entity

        builder
            .HasOne(u => u.CreatedByUser)
            .WithMany()
            .HasForeignKey(u => u.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(u => u.UpdatedByUser)
            .WithMany()
            .HasForeignKey(u => u.UpdatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // User -> Organization relationship is configured in OrganizationConfiguration

        // User -> Employee (optional link for Employee role users)
        builder
            .HasOne(u => u.Employee)
            .WithMany()
            .HasForeignKey(u => u.EmployeeId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder
            .HasIndex(u => new { u.OrganizationId, u.Email })
            .IsUnique()
            .HasFilter("is_deleted = false");
        builder
            .HasIndex(u => u.GoogleId)
            .IsUnique()
            .HasFilter("google_id IS NOT NULL AND is_deleted = false");
        builder.HasIndex(u => u.RefreshToken).HasFilter("refresh_token IS NOT NULL");
        builder
            .HasIndex(u => u.InvitationToken)
            .IsUnique()
            .HasFilter("invitation_token IS NOT NULL AND is_deleted = false");

        // Property configurations
        builder.Property(u => u.Email).HasMaxLength(255).IsRequired();
        builder.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(u => u.LastName).HasMaxLength(100).IsRequired();
        builder.Property(u => u.PhoneNumber).HasMaxLength(20);
        // Authentication credentials - at least one must be set (enforced by DB CHECK constraint)
        builder.Property(u => u.PasswordHash).HasMaxLength(500); // Optional for OAuth users
        builder.Property(u => u.GoogleId).HasMaxLength(100); // Optional for password users
        builder.Property(u => u.RefreshToken).HasMaxLength(500);
        builder.Property(u => u.PasswordResetToken).HasMaxLength(500);

        // Security stamp — rotated on every password reset to invalidate existing access tokens
        builder
            .Property(u => u.SecurityStamp)
            .HasDefaultValueSql("gen_random_uuid()")
            .IsRequired();

        // Invitation fields
        builder.Property(u => u.InvitationStatus).HasDefaultValue(InvitationStatus.None);
        builder.Property(u => u.InvitationToken).HasMaxLength(500);
    }
}
