using FairWorkly.Domain.Documents.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FairWorkly.Infrastructure.Persistence.Configurations.Documents;

/// <summary>
/// EF Core configuration for Document entity
/// </summary>
public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.HasKey(d => d.Id);

        // Configure AuditableEntity navigation properties
        builder
            .HasOne(d => d.CreatedByUser)
            .WithMany()
            .HasForeignKey(d => d.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(d => d.UpdatedByUser)
            .WithMany()
            .HasForeignKey(d => d.UpdatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Document -> Organization
        builder
            .HasOne(d => d.Organization)
            .WithMany()
            .HasForeignKey(d => d.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Document -> Employee (optional)
        builder
            .HasOne(d => d.Employee)
            .WithMany(e => e.Documents)
            .HasForeignKey(d => d.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(d => new { d.OrganizationId, d.DocumentType });
        builder.HasIndex(d => new { d.EmployeeId, d.DocumentType });
        builder.HasIndex(d => d.IsProvided);

        // Ignore computed properties
        builder.Ignore(d => d.IsCompliant);
        builder.Ignore(d => d.DaysUntilDeadline);

        // Property configurations
        builder.Property(d => d.ProvidedMethod).HasMaxLength(50);
        builder.Property(d => d.UploadedFileName).HasMaxLength(255);
        builder.Property(d => d.UploadedFilePath).HasMaxLength(500);
        builder.Property(d => d.Notes).HasMaxLength(1000);
    }
}
