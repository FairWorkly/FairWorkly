using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FairWorkly.Infrastructure.Persistence;

/// <summary>
/// Factory for creating DbContext at design time (for EF Core migrations).
/// Reads connection string from multiple sources in priority order:
/// 1. User Secrets (highest priority)
/// 2. appsettings.Development.json
/// 3. appsettings.json
/// 4. Environment variable FAIRWORKLY_CONNECTION_STRING (fallback)
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<FairWorklyDbContext>
{
    private const string UserSecretsId = "19188c8f-cabf-4ba8-ac6d-6c7f6d1db700";

    public FairWorklyDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        var connectionString = GetConnectionString(configuration);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string not found. Provide it via:\n" +
                "  1. User Secrets (ConnectionStrings:DefaultConnection)\n" +
                "  2. appsettings.json (ConnectionStrings:DefaultConnection)\n" +
                "  3. Environment variable FAIRWORKLY_CONNECTION_STRING"
            );
        }

        var optionsBuilder = new DbContextOptionsBuilder<FairWorklyDbContext>();
        optionsBuilder.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();

        return new FairWorklyDbContext(optionsBuilder.Options);
    }

    private static IConfiguration BuildConfiguration()
    {
        var apiProjectPath = GetApiProjectPath();

        var builder = new ConfigurationBuilder();

        // Environment variables (lowest priority, added first)
        builder.AddEnvironmentVariables();

        // appsettings.json from API project
        var appSettingsPath = Path.Combine(apiProjectPath, "appsettings.json");
        if (File.Exists(appSettingsPath))
        {
            builder.AddJsonFile(appSettingsPath, optional: true, reloadOnChange: false);
        }

        // Environment-specific appsettings
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var envSettingsPath = Path.Combine(apiProjectPath, $"appsettings.{environment}.json");
        if (File.Exists(envSettingsPath))
        {
            builder.AddJsonFile(envSettingsPath, optional: true, reloadOnChange: false);
        }

        // User Secrets (highest priority, added last)
        builder.AddUserSecrets(userSecretsId: UserSecretsId, reloadOnChange: false);

        return builder.Build();
    }

    private static string GetApiProjectPath()
    {
        // Design-time factory runs from Infrastructure project directory
        var currentDir = Directory.GetCurrentDirectory();

        // Navigate to sibling FairWorkly.API project
        var apiProjectPath = Path.GetFullPath(
            Path.Combine(currentDir, "..", "FairWorkly.API")
        );

        // Validate path exists (helpful for debugging)
        if (!Directory.Exists(apiProjectPath))
        {
            // Try alternate: maybe running from solution root
            var alternatePath = Path.GetFullPath(
                Path.Combine(currentDir, "src", "FairWorkly.API")
            );
            if (Directory.Exists(alternatePath))
            {
                return alternatePath;
            }
        }

        return apiProjectPath;
    }

    private static string? GetConnectionString(IConfiguration configuration)
    {
        // Primary: from configuration (User Secrets / appsettings)
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // Fallback: explicit environment variable (backward compatibility)
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = Environment.GetEnvironmentVariable("FAIRWORKLY_CONNECTION_STRING");
        }

        return connectionString;
    }
}
