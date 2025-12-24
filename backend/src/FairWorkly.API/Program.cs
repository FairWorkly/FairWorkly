using FairWorkly.API.ExceptionHandlers;
using FairWorkly.Application;
using FairWorkly.Infrastructure;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Diagnostics;

namespace FairWorkly.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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
            var frontendUrl = "http://localhost:5173";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllowFrontend",
                    policy =>
                    {
                        policy
                            .WithOrigins(frontendUrl)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    }
                );
            });

            // Configure JWT authentication
            var jwtSection = builder.Configuration.GetSection("JwtSettings");
            var jwtSecret = jwtSection["Secret"] ?? builder.Configuration["JwtSettings:Secret"];
            var jwtIssuer = jwtSection["Issuer"] ?? builder.Configuration["JwtSettings:Issuer"];
            var jwtAudience = jwtSection["Audience"] ?? builder.Configuration["JwtSettings:Audience"];

            var key = !string.IsNullOrEmpty(jwtSecret) ? new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)) : null;

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
                    ValidateIssuerSigningKey = key != null,
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
                        var handler =
                            context.RequestServices.GetRequiredService<IExceptionHandler>();
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
            app.UseCors("AllowFrontend");

            app.UseHttpsRedirection();

            // Authentication must come before Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            await app.RunAsync();
        }
    }
}
