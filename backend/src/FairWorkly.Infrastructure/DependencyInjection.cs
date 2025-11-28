using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Domain.Employees.Interfaces;
using FairWorkly.Infrastructure.AI.Mocks;
using FairWorkly.Infrastructure.AI.PythonServices;
using FairWorkly.Infrastructure.Persistence;
using FairWorkly.Infrastructure.Repositories.Employees;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        return services;
    }
}