using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Models;
using NotificationService.Infrastructure.Outbox;
namespace NotificationService.Infrastructure.Data
{
    public class NotificationDbContextFactory : IDesignTimeDbContextFactory<NotificationDbContext>
    {
        public NotificationDbContext CreateDbContext(string[] args)
        {
            // Get the configuration from appsettings.json
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "NotificationService.API");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var builder = new DbContextOptionsBuilder<NotificationDbContext>();
            var connectionString = configuration.GetConnectionString("NotificationConnection");

            builder.UseSqlServer(connectionString);

            return new NotificationDbContext(builder.Options);
        }
    }
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationTemplate> NotificationTemplates { get; set; }
        public DbSet<OutboxMessage> OutboxMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RecipientId).IsRequired();
                entity.Property(e => e.Type).IsRequired();
                entity.Property(e => e.Priority).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();

                entity.OwnsOne(e => e.Content, content =>
                {
                    content.Property(c => c.Title).IsRequired();
                    content.Property(c => c.Body).IsRequired();
                    content.Property(c => c.Metadata).HasConversion(
                        v => SerializeMetadata(v),
                        v => DeserializeMetadata(v));
                });

                entity.Property(e => e.SentVia).HasConversion(
                    v => string.Join(',', v.Select(c => (int)c)),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => (NotificationChannel)int.Parse(s))
                        .ToList());
            });

            modelBuilder.Entity<NotificationTemplate>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Type).IsRequired();
                entity.Property(e => e.TitleTemplate).IsRequired();
                entity.Property(e => e.BodyTemplate).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();

                entity.Property(e => e.DefaultMetadata).HasConversion(
                    v => SerializeMetadata(v),
                    v => DeserializeMetadata(v));
            });

            modelBuilder.Entity<OutboxMessage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EventType).IsRequired();
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.ProcessedAt);
                entity.Property(e => e.Error);
            });
        }

        private static string SerializeMetadata(Dictionary<string, string> metadata)
        {
            if (metadata == null || !metadata.Any())
                return "{}";

            return System.Text.Json.JsonSerializer.Serialize(metadata);
        }

        private static Dictionary<string, string> DeserializeMetadata(string json)
        {
            if (string.IsNullOrEmpty(json))
                return new Dictionary<string, string>();

            return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                ?? new Dictionary<string, string>();
        }
    }
}
