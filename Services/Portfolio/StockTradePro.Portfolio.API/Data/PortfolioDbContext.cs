using Microsoft.EntityFrameworkCore;
using StockTradePro.Portfolio.API.Models.Entities;

namespace StockTradePro.Portfolio.API.Data
{
    public class PortfolioDbContext : DbContext
    {
        public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options) : base(options) { }

        public DbSet<Models.Entities.Portfolio> Portfolios { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Holding> Holdings { get; set; }
        public DbSet<PortfolioPerformance> PortfolioPerformances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Portfolio entity
            modelBuilder.Entity<Models.Entities.Portfolio>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UserId).IsRequired().HasMaxLength(450);
                entity.Property(e => e.Type).HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(500);

                // Precision for decimal properties
                entity.Property(e => e.InitialValue).HasPrecision(18, 2);
                entity.Property(e => e.CurrentValue).HasPrecision(18, 2);
                entity.Property(e => e.TotalGainLoss).HasPrecision(18, 2);
                entity.Property(e => e.TotalGainLossPercent).HasPrecision(8, 4);

                // Indexes
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => new { e.UserId, e.IsActive });

                // Default values
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsPublic).HasDefaultValue(false);
                entity.Property(e => e.Type).HasDefaultValue("General");
            });

            // Configure Transaction entity
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Symbol).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Type).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Notes).HasMaxLength(500);

                // Precision for decimal properties
                entity.Property(e => e.Price).HasPrecision(18, 4);
                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
                entity.Property(e => e.Fees).HasPrecision(18, 2);
                entity.Property(e => e.NetAmount).HasPrecision(18, 2);

                // Relationships
                entity.HasOne(e => e.Portfolio)
                      .WithMany(p => p.Transactions)
                      .HasForeignKey(e => e.PortfolioId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Indexes
                entity.HasIndex(e => e.PortfolioId);
                entity.HasIndex(e => e.Symbol);
                entity.HasIndex(e => e.TransactionDate);
                entity.HasIndex(e => new { e.PortfolioId, e.Symbol });
            });

            // Configure Holding entity
            modelBuilder.Entity<Holding>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Symbol).IsRequired().HasMaxLength(10);
                entity.Property(e => e.CompanyName).HasMaxLength(200);

                // Precision for decimal properties
                entity.Property(e => e.AverageCost).HasPrecision(18, 4);
                entity.Property(e => e.TotalCost).HasPrecision(18, 2);
                entity.Property(e => e.CurrentPrice).HasPrecision(18, 4);
                entity.Property(e => e.CurrentValue).HasPrecision(18, 2);
                entity.Property(e => e.UnrealizedGainLoss).HasPrecision(18, 2);
                entity.Property(e => e.UnrealizedGainLossPercent).HasPrecision(8, 4);

                // Relationships
                entity.HasOne(e => e.Portfolio)
                      .WithMany(p => p.Holdings)
                      .HasForeignKey(e => e.PortfolioId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Indexes and constraints
                entity.HasIndex(e => e.PortfolioId);
                entity.HasIndex(e => e.Symbol);
                entity.HasIndex(e => new { e.PortfolioId, e.Symbol }).IsUnique(); // One holding per symbol per portfolio
            });

            // Configure PortfolioPerformance entity
            modelBuilder.Entity<PortfolioPerformance>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Precision for decimal properties
                entity.Property(e => e.TotalValue).HasPrecision(18, 2);
                entity.Property(e => e.DayChange).HasPrecision(18, 2);
                entity.Property(e => e.DayChangePercent).HasPrecision(8, 4);
                entity.Property(e => e.TotalReturn).HasPrecision(18, 2);
                entity.Property(e => e.TotalReturnPercent).HasPrecision(8, 4);
                entity.Property(e => e.CashValue).HasPrecision(18, 2);
                entity.Property(e => e.MarketValue).HasPrecision(18, 2);

                // Relationships
                entity.HasOne(e => e.Portfolio)
                      .WithMany(p => p.PerformanceHistory)
                      .HasForeignKey(e => e.PortfolioId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Indexes
                entity.HasIndex(e => e.PortfolioId);
                entity.HasIndex(e => e.Date);
                entity.HasIndex(e => new { e.PortfolioId, e.Date }).IsUnique(); // One performance record per portfolio per date
            });
        }
    }
}