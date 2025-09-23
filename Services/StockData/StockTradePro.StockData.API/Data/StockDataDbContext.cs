using Microsoft.EntityFrameworkCore;
using StockTradePro.StockData.API.Models;
namespace StockTradePro.StockData.API.Data
{
    public class StockDataDbContext : DbContext
    {
        public StockDataDbContext(DbContextOptions<StockDataDbContext> options) : base(options) { }

        public DbSet<Stock> Stocks { get; set; }
        public DbSet<StockPrice> StockPrices { get; set; }
        public DbSet<MarketData> MarketData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Stock entity
            modelBuilder.Entity<Stock>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Symbol).IsRequired().HasMaxLength(10);
                entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Exchange).HasMaxLength(50);
                entity.Property(e => e.Sector).HasMaxLength(100);
                entity.Property(e => e.Industry).HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Website).HasMaxLength(200);
                entity.Property(e => e.LogoUrl).HasMaxLength(200);
                entity.Property(e => e.MarketCap).HasPrecision(18, 2);

                // Unique constraint on Symbol
                entity.HasIndex(e => e.Symbol).IsUnique();

                // Static default values only
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                // Remove dynamic DateTime defaults - handle in model constructors instead
            });

            // Configure StockPrice entity
            modelBuilder.Entity<StockPrice>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OpenPrice).HasPrecision(18, 4);
                entity.Property(e => e.HighPrice).HasPrecision(18, 4);
                entity.Property(e => e.LowPrice).HasPrecision(18, 4);
                entity.Property(e => e.ClosePrice).HasPrecision(18, 4);
                entity.Property(e => e.CurrentPrice).HasPrecision(18, 4);
                entity.Property(e => e.PriceChange).HasPrecision(18, 4);
                entity.Property(e => e.PriceChangePercent).HasPrecision(8, 4);

                // Relationship with Stock
                entity.HasOne(e => e.Stock)
                      .WithMany(s => s.StockPrices)
                      .HasForeignKey(e => e.StockId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Indexes for performance
                entity.HasIndex(e => e.StockId);
                entity.HasIndex(e => e.Date);
                entity.HasIndex(e => e.IsCurrentPrice);
                entity.HasIndex(e => new { e.StockId, e.Date });

                // Static default values only
                entity.Property(e => e.IsCurrentPrice).HasDefaultValue(false);
                // Remove dynamic DateTime defaults - handle in model constructors instead
            });

            // Configure MarketData entity
            modelBuilder.Entity<MarketData>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.IndexName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.IndexSymbol).IsRequired().HasMaxLength(10);
                entity.Property(e => e.CurrentValue).HasPrecision(18, 4);
                entity.Property(e => e.PreviousClose).HasPrecision(18, 4);
                entity.Property(e => e.Change).HasPrecision(18, 4);
                entity.Property(e => e.ChangePercent).HasPrecision(8, 4);

                // Unique constraint on IndexSymbol
                entity.HasIndex(e => e.IndexSymbol).IsUnique();

                // Static default values only
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                // Remove dynamic DateTime defaults - handle in model constructors instead
            });
        }
    }
}