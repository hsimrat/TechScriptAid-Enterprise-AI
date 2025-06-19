using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection;
using TechScriptAid.Core.Entities;

namespace TechScriptAid.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly string _currentUser;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            // In a real application, this would come from IHttpContextAccessor or similar
            _currentUser = "System";
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, string currentUser)
            : base(options)
        {
            _currentUser = currentUser ?? "System";
        }

        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentAnalysis> DocumentAnalyses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations from assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Global configurations
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Configure decimal properties
                var decimalProperties = entityType.ClrType.GetProperties()
                    .Where(p => p.PropertyType == typeof(decimal) || p.PropertyType == typeof(decimal?));

                foreach (var property in decimalProperties)
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property(property.Name)
                        .HasPrecision(18, 4);
                }
            }
        }

        public override int SaveChanges()
        {
            OnBeforeSaving();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            OnBeforeSaving();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity &&
                    (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted));

            var currentTime = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                var entity = (BaseEntity)entry.Entity;

                switch (entry.State)
                {
                    case EntityState.Added:
                        entity.CreatedAt = currentTime;
                        entity.CreatedBy = _currentUser;
                        entity.UpdatedAt = currentTime;
                        entity.UpdatedBy = _currentUser;
                        entity.IsDeleted = false;
                        break;

                    case EntityState.Modified:
                        entity.UpdatedAt = currentTime;
                        entity.UpdatedBy = _currentUser;
                        // Ensure CreatedAt/CreatedBy are not modified
                        entry.Property(nameof(BaseEntity.CreatedAt)).IsModified = false;
                        entry.Property(nameof(BaseEntity.CreatedBy)).IsModified = false;
                        break;

                    case EntityState.Deleted:
                        // Implement soft delete
                        entry.State = EntityState.Modified;
                        entity.IsDeleted = true;
                        entity.DeletedAt = currentTime;
                        entity.DeletedBy = _currentUser;
                        entity.UpdatedAt = currentTime;
                        entity.UpdatedBy = _currentUser;
                        break;
                }
            }
        }

        // Method to permanently delete soft-deleted records (use with caution)
        public void PermanentlyDelete<T>(T entity) where T : BaseEntity
        {
            base.Remove(entity);
        }

        // Method to restore soft-deleted records
        public void Restore<T>(T entity) where T : BaseEntity
        {
            entity.IsDeleted = false;
            entity.DeletedAt = null;
            entity.DeletedBy = null;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = _currentUser;
        }
    }
}