using Microsoft.EntityFrameworkCore;
using VehicleService.Domain.Models;

namespace VehicleService.Infrastructure.Data
{
    public class VehicleDbContext : DbContext
    {
        public VehicleDbContext(DbContextOptions<VehicleDbContext> options) : base(options)
        {
        }

        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<MaintenanceRecord> MaintenanceRecords { get; set; }
        public DbSet<FuelRecord> FuelRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RegistrationNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Model).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Manufacturer).IsRequired().HasMaxLength(100);
                entity.Property(e => e.VIN).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Year).IsRequired();
                entity.OwnsOne(e => e.LastKnownLocation, loc =>
                {
                    loc.Property(p => p.Latitude).HasColumnName("LastLatitude");
                    loc.Property(p => p.Longitude).HasColumnName("LastLongitude");
                    loc.Property(p => p.Speed).HasColumnName("LastSpeed");
                    loc.Property(p => p.Heading).HasColumnName("LastHeading");
                    loc.Property(p => p.Timestamp).HasColumnName("LastLocationTimestamp");
                });

                entity.HasMany(e => e.MaintenanceHistory)
                    .WithOne()
                    .HasForeignKey(e => e.VehicleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.FuelHistory)
                    .WithOne()
                    .HasForeignKey(e => e.VehicleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<MaintenanceRecord>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Technician).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<FuelRecord>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Station).HasMaxLength(100);
                entity.Property(e => e.DriverName).HasMaxLength(100);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
