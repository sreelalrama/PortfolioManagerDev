using Microsoft.EntityFrameworkCore;
using StockTradePro.WatchlistService.API.Models; // Make sure this using is present

namespace StockTradePro.WatchlistService.API.Data
{
    public class WatchlistDbContext : DbContext
    {
        public WatchlistDbContext(DbContextOptions<WatchlistDbContext> options) : base(options)
        {
        }

        // Use fully qualified names to avoid conflict
        public DbSet<Models.Watchlist> Watchlists { get; set; }
        public DbSet<Models.WatchlistItem> WatchlistItems { get; set; }
        public DbSet<Models.PriceAlert> PriceAlerts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Watchlist configuration
            modelBuilder.Entity<Models.Watchlist>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UserId).IsRequired();
                entity.HasIndex(e => new { e.UserId, e.Name }).IsUnique();

                entity.HasMany(e => e.Items)
                    .WithOne(e => e.Watchlist)
                    .HasForeignKey(e => e.WatchlistId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.PriceAlerts)
                    .WithOne(e => e.Watchlist)
                    .HasForeignKey(e => e.WatchlistId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // WatchlistItem configuration
            modelBuilder.Entity<Models.WatchlistItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Symbol).IsRequired().HasMaxLength(10);
                entity.HasIndex(e => new { e.WatchlistId, e.Symbol }).IsUnique();
            });

            // PriceAlert configuration
            modelBuilder.Entity<Models.PriceAlert>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Symbol).IsRequired().HasMaxLength(10);
                entity.Property(e => e.TargetValue).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Type).HasConversion<string>();
                entity.Property(e => e.Status).HasConversion<string>();
            });
        }
    }
}