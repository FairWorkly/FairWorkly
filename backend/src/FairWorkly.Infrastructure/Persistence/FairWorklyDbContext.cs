using System.Reflection;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Employees.Entities;
using FairWorkly.Domain.Payroll.Entities;
using Microsoft.EntityFrameworkCore;

namespace FairWorkly.Infrastructure.Persistence
{
    public class FairWorklyDbContext : DbContext
    {
        public FairWorklyDbContext(DbContextOptions<FairWorklyDbContext> options)
            : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Payslip> Payslips { get; set; }
        public DbSet<PayrollValidation> PayrollValidations { get; set; }
        public DbSet<PayrollIssue> PayrollIssues { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Automatically load all configurations under the current assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default
        )
        {
            var now = DateTimeOffset.UtcNow;

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                }
            }

            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = now;
                }
            }

            // TODO: Add CreatedByUserId/UpdatedByUserId after JWT auth is implemented

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
