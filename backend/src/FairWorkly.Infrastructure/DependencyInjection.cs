using Amazon.S3;
using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Application.Employees.Interfaces;
using FairWorkly.Application.Payroll.Interfaces;
using FairWorkly.Application.Roster.Interfaces;
using FairWorkly.Domain.Auth.Interfaces;
using FairWorkly.Infrastructure.AI.Mocks;
using FairWorkly.Infrastructure.AI.PythonServices;
using FairWorkly.Infrastructure.Identity;
using FairWorkly.Infrastructure.Persistence;
using FairWorkly.Infrastructure.Persistence.Repositories.Auth;
using FairWorkly.Infrastructure.Persistence.Repositories.Employees;
using FairWorkly.Infrastructure.Persistence.Repositories.Payroll;
using FairWorkly.Infrastructure.Persistence.Repositories.Roster;
using FairWorkly.Infrastructure.Services;
using FairWorkly.Infrastructure.Storage;
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
        services.AddScoped<IRosterRepository, RosterRepository>();
        services.AddScoped<IRosterValidationRepository, RosterValidationRepository>();
        services.AddScoped<IPayrollValidationRepository, PayrollValidationRepository>();
        services.AddScoped<IPayslipRepository, PayslipRepository>();
        services.AddScoped<IPayrollIssueRepository, PayrollIssueRepository>();

        // Register UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register CurrentUserService
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // Register DateTimeProvider
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        // Register CurrentUserService (reads JWT claims from HttpContext)
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // Register FileStorageService based on configuration
        // Default to local storage when the key is missing so devs don't need AWS credentials
        var useS3 = configuration.GetValue<bool>("AWS:S3:Enabled");

        if (useS3)
        {
            // Use AWS S3 for production
            services.AddDefaultAWSOptions(configuration.GetAWSOptions());
            services.AddAWSService<IAmazonS3>();
            services.AddScoped<IFileStorageService, Storage.S3FileStorageService>();
        }
        else
        {
            // Use local file storage for development/testing
            services.AddScoped<IFileStorageService, Services.LocalFileStorageService>();
        }

        return services;
    }
}
