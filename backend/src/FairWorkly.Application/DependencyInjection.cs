using FairWorkly.Application.Common.Behaviors;
using FairWorkly.Application.Compliance.Orchestrators;
using FairWorkly.Application.Employees.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace FairWorkly.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register MediatR
        // Automatically scan all Command/Query Handlers
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

            // Registration Validation Pipeline
            // Run ValidationBehavior before executing any Handler
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });


        // Register FluentValidation
        // Automatically scan and register all Validators under the current assembly
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());


        // Register Services
        services.AddScoped<IEmployeeService, EmployeeService>();


        // Register AI Orchestrators
        services.AddScoped<ComplianceAiOrchestrator>();
        // TODO: 未来添加其他 Orchestrator 时在这里注册：
        // services.AddScoped<DocumentAiOrchestrator>();
        // services.AddScoped<PayrollAiOrchestrator>();
        // services.AddScoped<EmployeeAiOrchestrator>();

        return services;
    }
}