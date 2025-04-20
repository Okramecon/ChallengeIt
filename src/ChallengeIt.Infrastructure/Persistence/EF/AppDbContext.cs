using ChallengeIt.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChallengeIt.Infrastructure
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Challenge> Challenges { get; set; }
        public DbSet<CheckIn> CheckIns { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Challenge>()
                .HasMany(c => c.CheckIns)
                .WithOne(ci => ci.Challenge)
                .HasForeignKey(ci => ci.ChallengeId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Challenges)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId);

            modelBuilder.Entity<CheckIn>()
                .HasOne(ci => ci.User)
                .WithMany(c => c.CheckIns)
                .HasForeignKey(ci => ci.UserId);
            
            modelBuilder.Entity<CheckIn>()
                .HasIndex(ci => ci.TimeZoneId);
        }
    }
}
