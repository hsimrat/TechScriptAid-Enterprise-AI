using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TechScriptAid.Core.Entities;

namespace TechScriptAid.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentAnalysis> DocumentAnalyses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Document entity
            modelBuilder.Entity<Document>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Status).HasConversion<string>();

                // Store Tags as JSON
                entity.Property(e => e.Tags)
                    .HasConversion(
                        v => string.Join(',', v),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                    );

                // Index for performance
                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.CreatedAt);

                // Soft delete filter
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // Configure DocumentAnalysis entity
            modelBuilder.Entity<DocumentAnalysis>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Document)
                    .WithMany(d => d.Analyses)
                    .HasForeignKey(e => e.DocumentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.AnalysisType).HasConversion<string>();

                // Store Results as JSON
                entity.Property(e => e.Results).HasColumnType("nvarchar(max)");

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // Seed initial data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var documentId = Guid.NewGuid();
            modelBuilder.Entity<Document>().HasData(
                new Document
                {
                    Id = documentId,
                    Title = "Welcome to TechScriptAid Enterprise AI",
                    Content = "This is a sample document demonstrating our enterprise application structure.",
                    Category = "Tutorial",
                    Status = DocumentStatus.Published,
                    CreatedAt = DateTime.UtcNow,
                    Tags = new List<string> { "welcome", "tutorial", "enterprise" }
                }
            );
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.Id = Guid.NewGuid();
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
