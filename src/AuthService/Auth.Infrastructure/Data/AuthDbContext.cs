using Auth.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Data
{
    public class AuthDbContext : IdentityDbContext<AppUser>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
