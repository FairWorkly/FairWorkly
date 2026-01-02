using System.Reflection;
using FairWorkly.Application.Common.Behaviors;
using FairWorkly.Application.Compliance.Orchestrators;
using FairWorkly.Application.Documents.Interfaces;
using FairWorkly.Application.Documents.Orchestrators;
using FairWorkly.Application.Documents.Services;
using FairWorkly.Application.Employees.Interfaces;
using FairWorkly.Application.Employees.Orchestrators;
using FairWorkly.Application.Employees.Services;
using FairWorkly.Application.Payroll.Interfaces;
using FairWorkly.Application.Payroll.Services;
using FairWorkly.Application.Payroll.Services.ComplianceEngine;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

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
        services.AddScoped<IDocumentService, DocumentService>();

        // Register Payroll Services
        services.AddScoped<ICsvParserService, CsvParserService>();
        services.AddScoped<IEmployeeSyncService, EmployeeSyncService>();

        // Register Compliance Rules
        services.AddScoped<IComplianceRule, BaseRateRule>();
        services.AddScoped<IComplianceRule, PenaltyRateRule>();
        services.AddScoped<IComplianceRule, CasualLoadingRule>();
        services.AddScoped<IComplianceRule, SuperannuationRule>();

        // Register AI Orchestrators
        services.AddScoped<ComplianceAiOrchestrator>();
        services.AddScoped<DocumentAiOrchestrator>();
        services.AddScoped<EmployeeAiOrchestrator>();

        return services;
    }
}
