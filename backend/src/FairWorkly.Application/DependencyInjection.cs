using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;

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

            // TODO: 后续 Task 3 的 ValidationBehavior 会在这里注册
            // cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        // Register FluentValidation
        // Automatically scan and register all Validators under the current assembly
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}