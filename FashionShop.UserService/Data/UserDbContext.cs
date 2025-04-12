using FashionShop.UserService.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FashionShop.UserService.Data
{
    public class UserDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public UserDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.UserName).IsRequired();
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Email).IsRequired();
            });
            modelBuilder.Entity<IdentityRole<Guid>>(entity =>
            {
                entity.ToTable("Roles");
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired();
            });
            modelBuilder.Entity<IdentityUserRole<Guid>>(entity =>
            {
                entity.ToTable("UserRoles");
                entity.HasKey(e => new { e.UserId, e.RoleId });
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.RoleId).IsRequired();
            });

            //SeedingData(modelBuilder);
        }

        private void SeedingData(ModelBuilder modelBuilder)
        {
            // Define roles with fixed GUIDs
            var adminRole = new IdentityRole<Guid>
            {
                Id = Guid.Parse("d2b7f5e1-5c3b-4b8e-8b8e-8b8e8b8e8b8e"),
                Name = "Admin",
                NormalizedName = "ADMIN"
            };

            var userRole = new IdentityRole<Guid>
            {
                Id = Guid.Parse("e3c8f6f2-6d4c-4c9f-9c9f-9c9f9c9f9c9f"),
                Name = "User",
                NormalizedName = "USER"
            };

            modelBuilder.Entity<IdentityRole<Guid>>().HasData(adminRole, userRole);

            // Create admin user with pre-hashed password
            var adminUser = new User
            {
                Id = Guid.Parse("6567d753-cb38-460d-9f16-a1d89fe4fca4"),
                UserName = "dogiap",
                NormalizedUserName = "DOGIAP",
                Email = "ddooxhey@gmail.com",
                NormalizedEmail = "DDOOXHEY@GMAIL.COM",
                EmailConfirmed = true,
                SecurityStamp = "7f434309-a4d9-48e9-9ebb-8803db794577",
                // Pre-hashed password "123456" - DO NOT generate dynamically
                PasswordHash = "AQAAAAEAACcQAAAAEPYVaGvVpfYZQx/5UcI2zZp4Cz8Aq0+DBAi7tdnTGy61NlYACE3DcFKRt5OZZs3GXw=="
            };

            modelBuilder.Entity<User>().HasData(adminUser);

            // Add role to admin user
            var adminUserRole = new IdentityUserRole<Guid>
            {
                UserId = adminUser.Id,
                RoleId = adminRole.Id
            };

            modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(adminUserRole);
        }
    }
}
