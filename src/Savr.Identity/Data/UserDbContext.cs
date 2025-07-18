using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Savr.Domain.Entities;
using Savr.Identity.Models;

namespace Savr.Identity.Data
{
    public class UserDbContext : IdentityDbContext<ApplicationUser>
    {

        public DbSet<RefreshToken> RefreshTokens { get; set; } = default!;

        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var adminRoleId = "4A09F2F2-0D33-4C1A-9E3A-5D1A2A97FCF5";
            var userRoleId = "67D1B924-F5A6-4D2F-B6A0-1D10D2037991";
            var merchantRoleId = "C8419DF1-3F2E-4E7E-81D4-C8D48B3A2921";
            var adminUserId = "3d32337a-7372-4261-98b9-8352c83d8751";

            // Seed Roles
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Id = userRoleId,
                    Name = "User",
                    NormalizedName = "USER"
                },
                new IdentityRole
                {
                    Id = merchantRoleId,
                    Name = "Merchant",
                    NormalizedName = "MERCHANT"
                }
            );

            // (Optional) Seed Admin User (no password hash, just for reference)
            var hasher = new PasswordHasher<ApplicationUser>();
            var adminUser = new ApplicationUser
            {
                Id = adminUserId,
                UserName = "maahyarazad",
                NormalizedUserName = "MAAHYARAZAD",
                Email = "maahyarazad@gmail.com",
                NormalizedEmail = "MAAHYARAZAD@GMAIL.COM",
                EmailConfirmed = true,
                Firstname = "Maahyar",
                Lastname = "Azad",
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            adminUser.PasswordHash = hasher.HashPassword(adminUser, "StrongPassword123!");

            builder.Entity<ApplicationUser>().HasData(adminUser);

            // Assign Admin Role to User
            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = adminRoleId,
                UserId = adminUserId
            });


            builder.Entity<ApplicationUser>()
                .HasMany(x=>x.RefreshTokens)
                .WithOne(x=>x.User)
                .HasForeignKey(x=>x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

                        builder.Entity<ApplicationUser>()
                            .Navigation(nameof(ApplicationUser.RefreshTokens))
                            .UsePropertyAccessMode(PropertyAccessMode.Field);

                        builder.Entity<RefreshToken>()
                            .ToTable(nameof(RefreshToken));



        }
    }
}
