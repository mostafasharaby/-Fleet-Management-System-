using MaintenanceService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace MaintenanceService.Infrastructure.Data
{
    public class MaintenanceDbContext : DbContext
    {
        public DbSet<MaintenanceTask> MaintenanceTasks { get; set; }
        public DbSet<MaintenanceEvent> MaintenanceEvents { get; set; }
        public DbSet<PartReplacement> PartReplacements { get; set; }
        public DbSet<RequiredPart> RequiredParts { get; set; }

        public MaintenanceDbContext(DbContextOptions<MaintenanceDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MaintenanceTask>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Description).HasMaxLength(500).IsRequired();
                entity.Property(e => e.Type).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.EstimatedDurationMinutes).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();

                entity.HasMany(e => e.RequiredParts)
                      .WithOne()
                      .HasForeignKey("MaintenanceTaskId")
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Ignore(e => e.DomainEvents);
            });

            modelBuilder.Entity<MaintenanceEvent>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Description).HasMaxLength(500).IsRequired();
                entity.Property(e => e.Type).IsRequired();
                entity.Property(e => e.Timestamp).IsRequired();
                entity.Property(e => e.PerformedBy).HasMaxLength(100).IsRequired();
                entity.Property(e => e.OdometerReading).IsRequired();
                entity.Property(e => e.Notes).HasMaxLength(1000);

                entity.HasMany(e => e.PartsReplaced)
                      .WithOne()
                      .HasForeignKey("MaintenanceEventId")
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RequiredPart>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PartId).HasMaxLength(50).IsRequired();
                entity.Property(e => e.PartName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Quantity).IsRequired();
            });

            modelBuilder.Entity<PartReplacement>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PartId).HasMaxLength(50).IsRequired();
                entity.Property(e => e.PartName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.Cost).IsRequired();
            });

        }
    }
}
