using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public static class SeedData
{
    public static void SeedRoles(ModelBuilder builder)
    {
        builder.Entity<IdentityRole>().HasData(
                 new IdentityRole
                 {
                     Id = "1",
                     Name = "Admin",
                     NormalizedName = "ADMIN",
                     ConcurrencyStamp = "admin-role-stamp-2024"  // Static value
                 },
                 new IdentityRole
                 {
                     Id = "2",
                     Name = "User",
                     NormalizedName = "USER",
                     ConcurrencyStamp = "user-role-stamp-2024"   // Static value
                 },
                 new IdentityRole
                 {
                     Id = "3",
                     Name = "Premium",
                     NormalizedName = "PREMIUM",
                     ConcurrencyStamp = "premium-role-stamp-2024" // Static value
                 }
         );
    }
}