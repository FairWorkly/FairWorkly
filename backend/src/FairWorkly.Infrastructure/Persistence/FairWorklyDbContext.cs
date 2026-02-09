using System.Linq.Expressions;
using System.Reflection;
using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Awards.Entities;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Documents.Entities;
using FairWorkly.Domain.Employees.Entities;
using FairWorkly.Domain.Payroll.Entities;
using FairWorkly.Domain.Roster.Entities;
using Microsoft.EntityFrameworkCore;

// All entity relationship configurations are in:
// Infrastructure/Persistence/Configurations/{Module}/*Configuration.cs

namespace FairWorkly.Infrastructure.Persistence
{
    public class FairWorklyDbContext : DbContext
    {
        public FairWorklyDbContext(DbContextOptions<FairWorklyDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<OrganizationAward> OrganizationAwards { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Award> Awards { get; set; }
        public DbSet<AwardLevel> AwardLevels { get; set; }

        public DbSet<Document> Documents { get; set; }

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

            // Automatically load all configurations from *Configuration.cs files
            // All entity relationships, indexes, and property configurations are defined there
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Global query filter for soft-deleted entities
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (!typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    continue;
                }

                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Call(
                    typeof(EF),
                    nameof(EF.Property),
                    new[] { typeof(bool) },
                    parameter,
                    Expression.Constant(nameof(BaseEntity.IsDeleted))
                );
                var filter = Expression.Lambda(Expression.Equal(property, Expression.Constant(false)), parameter);
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
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

            // Normalize User.Email on create/update (trim + lowercase for consistent lookups)
            foreach (var entry in ChangeTracker.Entries<User>())
            {
                if (entry.State is EntityState.Added or EntityState.Modified)
                {
                    if (!string.IsNullOrWhiteSpace(entry.Entity.Email))
                    {
                        entry.Entity.Email = entry.Entity.Email.Trim().ToLowerInvariant();
                    }
                }
            }

            // Domain validation fallback - catches any invalid entities before they hit the database
            // This is a safety net; Application layer validators should catch errors first for friendly messages
            foreach (var entry in ChangeTracker.Entries<IValidatableDomain>())
            {
                if (entry.State is EntityState.Added or EntityState.Modified)
                {
                    entry.Entity.ValidateDomainRules();
                }
            }

            // TODO: Add CreatedByUserId/UpdatedByUserId after JWT auth is implemented

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
