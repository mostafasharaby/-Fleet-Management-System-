using Auth.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Data
{
    public class AuthDbContext : IdentityDbContext<AppUser>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Permission configuration
            modelBuilder.Entity<Permission>().HasKey(p => p.Id);
            modelBuilder.Entity<Permission>().Property(p => p.Name).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Permission>().Property(p => p.Resource).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Permission>().Property(p => p.Action).IsRequired().HasMaxLength(100);

            base.OnModelCreating(modelBuilder);
        }
    }
}
