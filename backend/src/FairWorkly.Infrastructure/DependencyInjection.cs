using FairWorkly.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FairWorkly.Domain.Employees.Interfaces;
using FairWorkly.Infrastructure.Repositories.Employees;

namespace FairWorkly.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // Register DbContext (PostgreSQL)
        services.AddDbContext<FairWorklyDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Register Repositories
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();

        // TODO: AI Client (Task 4 会用到)
        // services.AddHttpClient<IAiClient, PythonAiClient>();

        return services;
    }
}