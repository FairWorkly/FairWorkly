using FairWorkly.API.ExceptionHandlers;
using FairWorkly.Application;
using FairWorkly.Infrastructure;
using FairWorkly.Infrastructure.Persistence;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
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
    builder.Services.AddSwaggerGen(c =>
    {
        // enable example filters from Swashbuckle.AspNetCore.Filters
        c.ExampleFilters();

        // Add Bearer auth to Swagger
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer {token}'",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                },
                new string[] { }
            }
        });
    });

    // Register example providers from this assembly (LoginCommandExample)
    builder.Services.AddSwaggerExamplesFromAssemblyOf<FairWorkly.API.SwaggerExamples.LoginCommandExample>();

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

    // Configure JWT authentication
    var jwtSection = builder.Configuration.GetSection("JwtSettings");
    var jwtSecret = jwtSection["Secret"] ?? builder.Configuration["JwtSettings:Secret"];
    if (string.IsNullOrWhiteSpace(jwtSecret) || jwtSecret == "REPLACE_IN_ENVIRONMENT")
    {
        throw new InvalidOperationException(
            "JwtSettings:Secret is missing. Set it via environment config (user-secrets/appsettings.Development.json)."
        );
    }
    var jwtIssuer = jwtSection["Issuer"] ?? builder.Configuration["JwtSettings:Issuer"];
    var jwtAudience = jwtSection["Audience"] ?? builder.Configuration["JwtSettings:Audience"];

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = !string.IsNullOrEmpty(jwtIssuer),
            ValidIssuer = jwtIssuer,
            ValidateAudience = !string.IsNullOrEmpty(jwtAudience),
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
        options.AddPolicy("RequireManager", policy => policy.RequireRole("Admin", "HrManager"));
        options.AddPolicy("EmployeeOnly", policy => policy.RequireRole("Employee"));
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
        await DbSeeder.SeedAsync(app);
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Must before UseAuthorization
    app.UseCors("AllowAll");

    app.UseHttpsRedirection();

    // Authentication must come before Authorization
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    await app.RunAsync();
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
