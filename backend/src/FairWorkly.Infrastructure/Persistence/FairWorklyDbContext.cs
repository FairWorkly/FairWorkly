using System.Reflection;
using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Compliance.Entities;
using FairWorkly.Domain.Employees.Entities;
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
        public DbSet<Roster> Rosters { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<RosterValidation> RosterValidations { get; set; }
        public DbSet<RosterIssue> RosterIssues { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Automatically load all configurations under the current assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Align with singular table names in migrations
            modelBuilder.Entity<User>().ToTable("user");
            modelBuilder.Entity<Organization>().ToTable("organization");

            modelBuilder
                .Entity<Organization>()
                .HasOne(organization => organization.CreatedByUser)
                .WithMany()
                .HasForeignKey(organization => organization.CreatedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
                .Entity<Organization>()
                .HasOne(organization => organization.UpdatedByUser)
                .WithMany()
                .HasForeignKey(organization => organization.UpdatedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
                .Entity<Employee>()
                .HasOne(employee => employee.CreatedByUser)
                .WithMany()
                .HasForeignKey(employee => employee.CreatedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
                .Entity<Employee>()
                .HasOne(employee => employee.UpdatedByUser)
                .WithMany()
                .HasForeignKey(employee => employee.UpdatedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
                .Entity<User>()
                .HasOne(user => user.CreatedByUser)
                .WithMany()
                .HasForeignKey(user => user.CreatedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
                .Entity<User>()
                .HasOne(user => user.UpdatedByUser)
                .WithMany()
                .HasForeignKey(user => user.UpdatedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
                .Entity<User>()
                .HasOne(user => user.Employee)
                .WithMany()
                .HasForeignKey(user => user.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
                .Entity<Shift>()
                .HasOne(shift => shift.Organization)
                .WithMany()
                .HasForeignKey(shift => shift.OrganizationId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
                .Entity<Shift>()
                .HasOne(shift => shift.Employee)
                .WithMany(e => e.Shifts)
                .HasForeignKey(shift => shift.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
                .Entity<RosterValidation>()
                .HasOne(rv => rv.CreatedByUser)
                .WithMany()
                .HasForeignKey(rv => rv.CreatedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
                .Entity<RosterValidation>()
                .HasOne(rv => rv.UpdatedByUser)
                .WithMany()
                .HasForeignKey(rv => rv.UpdatedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
                .Entity<RosterIssue>()
                .HasOne(ri => ri.Organization)
                .WithMany()
                .HasForeignKey(ri => ri.OrganizationId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
                .Entity<RosterIssue>()
                .HasOne(ri => ri.RosterValidation)
                .WithMany(rv => rv.Issues)
                .HasForeignKey(ri => ri.RosterValidationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<RosterIssue>()
                .HasOne(ri => ri.Shift)
                .WithMany(s => s.Issues)
                .HasForeignKey(ri => ri.ShiftId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
                .Entity<RosterIssue>()
                .HasOne(ri => ri.Employee)
                .WithMany()
                .HasForeignKey(ri => ri.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
                .Entity<RosterIssue>()
                .HasOne(ri => ri.ResolvedByUser)
                .WithMany()
                .HasForeignKey(ri => ri.ResolvedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
                .Entity<RosterIssue>()
                .HasOne(ri => ri.WaivedByUser)
                .WithMany()
                .HasForeignKey(ri => ri.WaivedByUserId)
                .OnDelete(DeleteBehavior.NoAction);
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
