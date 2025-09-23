using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StockTradePro.UserManagement.API.Models;

namespace StockTradePro.UserManagement.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<UserProfile> UserProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure ApplicationUser
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.FirstName).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);
                entity.Property(e => e.ProfileImageUrl).HasMaxLength(200);
                entity.Property(e => e.Bio).HasMaxLength(500);
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Configure UserProfile
            builder.Entity<UserProfile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Occupation).HasMaxLength(100);
                entity.Property(e => e.Company).HasMaxLength(100);
                entity.Property(e => e.Location).HasMaxLength(200);
                entity.Property(e => e.RiskTolerance).HasMaxLength(50);
                entity.Property(e => e.InvestmentExperience).HasMaxLength(50);
                entity.Property(e => e.AnnualIncome).HasPrecision(18, 2);

                // Relationship
                entity.HasOne(e => e.User)
                      .WithMany(u => u.Profiles)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed default roles
            SeedData.SeedRoles(builder);
        }
    }
}
