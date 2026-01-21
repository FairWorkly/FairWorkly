using System.Reflection;
using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Compliance.Entities;
using FairWorkly.Domain.Employees.Entities;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Payroll.Entities;
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

        public DbSet<Payslip> Payslips { get; set; }
        public DbSet<PayrollValidation> PayrollValidations { get; set; }
        public DbSet<PayrollIssue> PayrollIssues { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Automatically load all configurations under the current assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Align with singular table names in migrations
            modelBuilder.Entity<User>().ToTable("user");
            modelBuilder.Entity<Organization>().ToTable("organization");

            modelBuilder.Entity<User>().HasIndex(user => user.Email).IsUnique();

            modelBuilder.Entity<Organization>()
                .HasOne(organization => organization.CreatedByUser)
                .WithMany()
                .HasForeignKey(organization => organization.CreatedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Organization>()
                .HasOne(organization => organization.UpdatedByUser)
                .WithMany()
                .HasForeignKey(organization => organization.UpdatedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Employee>()
                .HasOne(employee => employee.CreatedByUser)
                .WithMany()
                .HasForeignKey(employee => employee.CreatedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Employee>()
                .HasOne(employee => employee.UpdatedByUser)
                .WithMany()
                .HasForeignKey(employee => employee.UpdatedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
                .HasOne(user => user.CreatedByUser)
                .WithMany()
                .HasForeignKey(user => user.CreatedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
                .HasOne(user => user.UpdatedByUser)
                .WithMany()
                .HasForeignKey(user => user.UpdatedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
                .HasOne(user => user.Employee)
                .WithMany()
                .HasForeignKey(user => user.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<RosterValidation>()
                .HasOne(rosterValidation => rosterValidation.Roster)
                .WithOne(roster => roster.RosterValidation)
                .HasForeignKey<RosterValidation>(rosterValidation => rosterValidation.RosterId)
                .OnDelete(DeleteBehavior.Cascade);
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
