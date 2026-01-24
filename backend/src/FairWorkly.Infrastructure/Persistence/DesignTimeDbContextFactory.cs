using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FairWorkly.Infrastructure.Persistence;

/// <summary>
/// Factory for creating DbContext at design time (for EF Core migrations)
/// Must match runtime configuration in DependencyInjection.cs
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<FairWorklyDbContext>
{
    public FairWorklyDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FairWorklyDbContext>();

        var connectionString = Environment.GetEnvironmentVariable("FAIRWORKLY_CONNECTION_STRING");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Set FAIRWORKLY_CONNECTION_STRING for design-time migrations."
            );
        }
        optionsBuilder.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();

        return new FairWorklyDbContext(optionsBuilder.Options);
    }
}
