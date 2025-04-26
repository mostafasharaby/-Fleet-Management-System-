using Microsoft.EntityFrameworkCore;
using TelemetryService.Domain.Models;

namespace TelemetryService.Infrastructure.Data
{
    public class TelemetryDbContext : DbContext
    {
        public TelemetryDbContext(DbContextOptions<TelemetryDbContext> options)
            : base(options)
        {
        }

        public DbSet<TelemetryData> TelemetryData { get; set; }
        public DbSet<AlertThreshold> AlertThresholds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TelemetryData>(entity =>
            {
                entity.ToTable("TelemetryData");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.VehicleId).IsRequired();
                entity.Property(e => e.Timestamp).IsRequired();
                entity.Property(e => e.DiagnosticCode).HasMaxLength(50);

                // Create indexes for efficient queries
                entity.HasIndex(e => e.VehicleId);
                entity.HasIndex(e => e.Timestamp);
                entity.HasIndex(e => new { e.VehicleId, e.Timestamp });
            });

            modelBuilder.Entity<AlertThreshold>(entity =>
            {
                entity.ToTable("AlertThresholds");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.VehicleId).IsRequired();
                entity.Property(e => e.ParameterName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.AlertMessage).HasMaxLength(200);

                entity.HasIndex(e => e.VehicleId);
            });
        }
    }
}