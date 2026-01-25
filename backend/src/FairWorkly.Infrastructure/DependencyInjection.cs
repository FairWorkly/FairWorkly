using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Domain.Auth.Interfaces;
using FairWorkly.Application.Employees.Interfaces;
using FairWorkly.Infrastructure.AI.Mocks;
using FairWorkly.Infrastructure.AI.PythonServices;
using FairWorkly.Infrastructure.Identity;
using FairWorkly.Infrastructure.Persistence;
using FairWorkly.Infrastructure.Persistence.Repositories.Auth;
using FairWorkly.Infrastructure.Persistence.Repositories.Employees;
using FairWorkly.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FairWorkly.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // true: use real AI service; false: use mock AI service
        var useMockAi = configuration.GetValue<bool>("AiSettings:UseMockAi");

        if (useMockAi)
        {
            services.AddSingleton<IAiClient, MockAiClient>();
        }
        else
        {
            services.AddHttpClient<IAiClient, PythonAiClient>();
        }

        // Register DbContext (PostgreSQL)
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        // UseSnakeCaseNamingConvention for PostgreSQL
        services.AddDbContext<FairWorklyDbContext>(options =>
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention()
        );

        // Register Auth Services
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();
        // Secret hasher for tokens (refresh/reset/api keys)
        services.AddScoped<ISecretHasher, SecretHasher>();

        // Register Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();

        // Register UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register DateTimeProvider
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        // Register FileStorageService
        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        return services;
    }
}
