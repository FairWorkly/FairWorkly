using FairWorkly.API.ExceptionHandlers;
using FairWorkly.Application;
using FairWorkly.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;

// ============ Serilog Configuration ============
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile(
        $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
        optional: true
    )
    .Build();

var loggerConfig = new LoggerConfiguration().ReadFrom.Configuration(configuration);

var fileLoggingEnabled = configuration.GetValue<bool>("FileLogging:Enabled");
if (fileLoggingEnabled)
{
    var logPath = configuration.GetValue<string>("FileLogging:Path") ?? "logs/fairworkly-.log";
    loggerConfig.WriteTo.File(
        path: logPath,
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
    );
}

Log.Logger = loggerConfig.CreateLogger();

// ============ Serilog Configuration End ============

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    // Register Application and Infrastructure services (DependencyInjection.cs)
    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructureServices(builder.Configuration);

    // Add controllers
    builder.Services.AddControllers();

    // Add Swagger generator
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Explicitly register Global Exception Handler
    builder.Services.AddSingleton<IExceptionHandler, GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    // Add CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(
            "AllowAll",
            policy =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                }
                else
                {
                    var allowedOrigins =
                        builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                        ?? Array.Empty<string>();
                    policy.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader();
                }
            }
        );
    });

    /* -------------------------------------- */
    /* app */
    var app = builder.Build();

    // Enable exception handling middleware
    // Must be placed at the front of the pipeline
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
            var exception = exceptionHandlerFeature?.Error;

            if (exception != null)
            {
                var handler = context.RequestServices.GetRequiredService<IExceptionHandler>();
                await handler.TryHandleAsync(context, exception, CancellationToken.None);
            }
        });
    });

    // Enable Swagger UI in Development
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Must before UseAuthorization
    app.UseCors("AllowAll");

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
