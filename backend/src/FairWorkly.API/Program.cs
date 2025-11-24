using FairWorkly.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FairWorkly.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add API explorer (Required for Swagger)
            builder.Services.AddEndpointsApiExplorer();

            // Add Swagger generator
            builder.Services.AddSwaggerGen();

            // Register DbContext
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<FairWorklyDbContext>(options => options.UseNpgsql(connectionString));

            // Add controllers
            builder.Services.AddControllers();

            /* -------------------------------------- */
            /* app */
            var app = builder.Build();

            // Enable Swagger UI in Development
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}

