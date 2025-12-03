using FairWorkly.API.ExceptionHandlers;
using FairWorkly.Application;
using FairWorkly.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;

namespace FairWorkly.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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
                        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
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
            app.UseCors("AllowAll");

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
