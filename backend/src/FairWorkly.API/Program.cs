using FairWorkly.Application;
using FairWorkly.Infrastructure;

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

            // Add CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            /* -------------------------------------- */
            /* app */
            var app = builder.Build();

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

            // TODO: 注册全局异常处理 (Task 3 会用到)
            // app.UseMiddleware<GlobalExceptionMiddleware>(); 

            app.MapControllers();

            app.Run();
        }
    }
}

