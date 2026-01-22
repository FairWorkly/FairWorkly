using FairWorkly.Domain.Awards.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FairWorkly.Infrastructure.Persistence.Configurations.Awards;

/// <summary>
/// EF Core configuration for Award entity
/// Note: Award inherits from BaseEntity (no audit properties)
/// </summary>
public class AwardConfiguration : IEntityTypeConfiguration<Award>
{
    public void Configure(EntityTypeBuilder<Award> builder)
    {
        builder.HasKey(a => a.Id);

        // Award -> AwardLevels (One-to-Many)
        builder.HasMany(a => a.Levels)
            .WithOne(l => l.Award)
            .HasForeignKey(l => l.AwardId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(a => a.AwardType).IsUnique();
        builder.HasIndex(a => a.AwardCode).IsUnique();

        // Property configurations
        builder.Property(a => a.Name).HasMaxLength(200).IsRequired();
        builder.Property(a => a.AwardCode).HasMaxLength(20).IsRequired();
        builder.Property(a => a.Description).HasMaxLength(1000);
        builder.Property(a => a.SaturdayPenaltyRate).HasPrecision(5, 4);
        builder.Property(a => a.SundayPenaltyRate).HasPrecision(5, 4);
        builder.Property(a => a.PublicHolidayPenaltyRate).HasPrecision(5, 4);
        builder.Property(a => a.CasualLoadingRate).HasPrecision(5, 4);
        builder.Property(a => a.MinimumShiftHours).HasPrecision(5, 2);
    }
}
