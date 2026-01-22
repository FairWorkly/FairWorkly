using FairWorkly.Domain.Awards.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FairWorkly.Infrastructure.Persistence.Configurations.Awards;

/// <summary>
/// EF Core configuration for AwardLevel entity
/// Note: AwardLevel inherits from BaseEntity (no audit properties)
/// </summary>
public class AwardLevelConfiguration : IEntityTypeConfiguration<AwardLevel>
{
    public void Configure(EntityTypeBuilder<AwardLevel> builder)
    {
        builder.HasKey(al => al.Id);

        // AwardLevel -> Award (configured in AwardConfiguration as well)
        builder.HasOne(al => al.Award)
            .WithMany(a => a.Levels)
            .HasForeignKey(al => al.AwardId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        // Active rate lookup: Award + Level + IsActive
        builder.HasIndex(al => new { al.AwardId, al.LevelNumber, al.IsActive });
        // Historical rate lookup: Award + Level + EffectiveFrom
        builder.HasIndex(al => new { al.AwardId, al.LevelNumber, al.EffectiveFrom });

        // Property configurations
        builder.Property(al => al.LevelName).HasMaxLength(100).IsRequired();
        builder.Property(al => al.Description).HasMaxLength(500);
        builder.Property(al => al.FullTimeHourlyRate).HasPrecision(10, 4);
        builder.Property(al => al.PartTimeHourlyRate).HasPrecision(10, 4);
        builder.Property(al => al.CasualHourlyRate).HasPrecision(10, 4);
    }
}
