using System.Reflection;
using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Employees.Entities;
using Microsoft.EntityFrameworkCore;

namespace FairWorkly.Infrastructure.Persistence
{
    public class FairWorklyDbContext : DbContext
    {
        public FairWorklyDbContext(DbContextOptions<FairWorklyDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Organization> Organizations { get; set; }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Automatically load all configurations under the current assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
