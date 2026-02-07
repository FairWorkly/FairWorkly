using System.Reflection;
using FairWorkly.Application.Common.Behaviors;
using FairWorkly.Application.Documents.Interfaces;
using FairWorkly.Application.Documents.Orchestrators;
using FairWorkly.Application.Documents.Services;
using FairWorkly.Application.Employees.Interfaces;
using FairWorkly.Application.Employees.Orchestrators;
using FairWorkly.Application.Employees.Services;
using FairWorkly.Application.Payroll.Interfaces;
using FairWorkly.Application.Payroll.Orchestrators;
using FairWorkly.Application.Payroll.Services;
using FairWorkly.Application.Payroll.Services.ComplianceEngine;
using FairWorkly.Application.Roster.Orchestrators;
using FairWorkly.Application.Roster.Services;
using FairWorkly.Domain.Roster.Parameters;
using FairWorkly.Domain.Roster.Rules;
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
        services.AddScoped<RosterAiOrchestrator>();
        services.AddScoped<PayrollAiOrchestrator>();
        services.AddScoped<DocumentAiOrchestrator>();
        services.AddScoped<EmployeeAiOrchestrator>();

        // Register Roster Compliance Engine + Rules
        // NOTE: Singleton is appropriate while parameters are static Award rules.
        // Change to Scoped if parameters need to vary per-tenant or read from DB.
        services.AddSingleton<IRosterRuleParametersProvider, AwardRosterRuleParametersProvider>();
        services.AddScoped<IRosterComplianceEngine, RosterComplianceEngine>();

        // DataQualityRule first - detects missing Employee data before other rules run
        services.AddScoped<IRosterComplianceRule, DataQualityRule>();
        services.AddScoped<IRosterComplianceRule, MinimumShiftHoursRule>();
        services.AddScoped<IRosterComplianceRule, MealBreakRule>();
        services.AddScoped<IRosterComplianceRule, RestPeriodRule>();
        services.AddScoped<IRosterComplianceRule, WeeklyHoursLimitRule>();
        services.AddScoped<IRosterComplianceRule, ConsecutiveDaysRule>();

        return services;
    }
}
