using Microsoft.EntityFrameworkCore;
using RouteService.Domain.Models;

namespace RouteService.Infrastructure.Data
{
    public class RouteDbContext : DbContext
    {
        public RouteDbContext(DbContextOptions<RouteDbContext> options) : base(options)
        {
        }

        public DbSet<Route> Routes { get; set; }
        public DbSet<RouteStop> RouteStops { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Route>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.VehicleId).IsRequired();
                entity.Property(e => e.DriverId).IsRequired();
                entity.Property(e => e.StartTime).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.HasMany(e => e.Stops)
                      .WithOne()
                      .HasForeignKey(e => e.RouteId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RouteStop>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RouteId).IsRequired();
                entity.Property(e => e.SequenceNumber).IsRequired();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Address).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Latitude).IsRequired();
                entity.Property(e => e.Longitude).IsRequired();
                entity.Property(e => e.PlannedArrivalTime).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.EstimatedDurationMinutes).IsRequired();
                entity.Property(e => e.Notes).HasMaxLength(500);
            });
        }
    }
}
