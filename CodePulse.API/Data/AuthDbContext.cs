using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CodePulse.API.Data;

public class AuthDbContext : IdentityDbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        const string readerRoleId = "f7def4cf-e8fe-4c78-b16e-cf77038f1f0b";
        const string writerRoleId = "296d0975-f787-4c95-b83b-cda761b98a2a";

        // Create Reader and Writer Roles
        var roles = new List<IdentityRole>
        {
            new IdentityRole()
            {
                Id = readerRoleId,
                Name = "Reader",
                NormalizedName = "Reader".ToUpper(),
                ConcurrencyStamp = readerRoleId
            },
            new IdentityRole()
            {
                Id = writerRoleId,
                Name = "Writer",
                NormalizedName = "Writer".ToUpper(),
                ConcurrencyStamp = writerRoleId
            }
        };

        // Seed the roles
        builder.Entity<IdentityRole>().HasData(roles);

        // Create an Admin User
        const string adminUserId = "88ab90eb-a76d-4c5f-9ab5-3b9b3a0f27f0";
        var admin = new IdentityUser()
        {
            Id = adminUserId,
            UserName = "admin@codepulse.com",
            Email = "admin@codepulse.com",
            NormalizedEmail = "admin@codepulse.com".ToUpper(),
            NormalizedUserName = "admin@codepulse.com".ToUpper(),
        };

        admin.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(admin, "Admin@123");

        builder.Entity<IdentityUser>().HasData(admin);

        // Give Roles to Admin
        var adminRoles = new List<IdentityUserRole<string>>()
        {
            new ()
            {
                UserId = adminUserId,
                RoleId = readerRoleId
            },
            new ()
            {
                UserId = adminUserId,
                RoleId = writerRoleId
            }
        };

        builder.Entity<IdentityUserRole<string>>().HasData(adminRoles);
    }


}