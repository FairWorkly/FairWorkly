using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FairWorkly.Infrastructure.Persistence;

/// <summary>
/// Factory for creating DbContext at design time (for EF Core migrations)
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<FairWorklyDbContext>
{
    public FairWorklyDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FairWorklyDbContext>();

        // Use a dummy connection string for design-time operations
        optionsBuilder.UseNpgsql(
            "Host=localhost;Database=FairWorklyDb;Username=postgres;Password=postgres"
        );

        return new FairWorklyDbContext(optionsBuilder.Options);
    }
}
