using DriverService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DriverService.Infrastructure.Data
{

    public class DriverDbContext : DbContext
    {
        public DriverDbContext(DbContextOptions<DriverDbContext> options) : base(options)
        {
        }

        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<ScheduleEntry> ScheduleEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Driver>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.LicenseNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LicenseState).IsRequired().HasMaxLength(50);

                entity.HasMany(e => e.Assignments)
                    .WithOne()
                    .HasForeignKey(e => e.DriverId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Schedule)
                    .WithOne()
                    .HasForeignKey(e => e.DriverId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Notes).HasMaxLength(500);
            });

            modelBuilder.Entity<ScheduleEntry>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Notes).HasMaxLength(500);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
