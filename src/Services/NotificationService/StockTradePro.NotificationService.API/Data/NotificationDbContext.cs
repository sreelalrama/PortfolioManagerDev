using Microsoft.EntityFrameworkCore;
using StockTradePro.NotificationService.API.Models;

namespace StockTradePro.NotificationService.API.Data
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
        {
        }

        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserNotificationPreference> UserNotificationPreferences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Notification configuration
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Message).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Type).HasConversion<string>();
                entity.Property(e => e.Status).HasConversion<string>();
                entity.Property(e => e.Method).HasConversion<string>();

                entity.HasIndex(e => new { e.UserId, e.CreatedAt });
                entity.HasIndex(e => new { e.Status, e.CreatedAt });
                entity.HasIndex(e => e.Type);
            });

            // UserNotificationPreference configuration
            modelBuilder.Entity<UserNotificationPreference>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.Type).HasConversion<string>();

                entity.HasIndex(e => new { e.UserId, e.Type }).IsUnique();
            });

            // Remove seed data that might be causing issues
            // SeedDefaultPreferences(modelBuilder);
        }
    }
}