using Microsoft.EntityFrameworkCore;
using Shopverse.Domain.Entities;
public static class DatabaseSeeder
{
    public static async Task SeedAsync(DbContext context)
    {


        await context.SaveChangesAsync();

        var adminRole = await context.Set<Role>().FirstOrDefaultAsync(r => r.Name == "Administrator");
        if (adminRole == null)
        {
            adminRole = new Role { Name = "Administrator", Key = "ADMINISTRATOR" };
            await context.Set<Role>().AddAsync(adminRole);
            await context.SaveChangesAsync();
        }

        var systemUser = await context.Set<User>().FirstOrDefaultAsync(u => u.Username == "System");
        if (systemUser == null)
        {
            systemUser = new User
            {
                Username = "System",
                Email = "system@pnu.local",
                IsActive = false
            };
            await context.Set<User>().AddAsync(systemUser);
            await context.SaveChangesAsync();
        }

        var adminUser = await context.Set<User>().FirstOrDefaultAsync(u => u.Email == "admin@pnu.local");
        if (adminUser == null)
        {
            adminUser = new User
            {
                Username = "admin",
                Email = "admin@pnu.local",
                IsActive = true,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123")
            };

            await context.Set<User>().AddAsync(adminUser);
            await context.SaveChangesAsync();

            var userRole = new UserRole
            {
                UserId = adminUser.Id,
                RoleId = adminRole.Id
            };

            await context.Set<UserRole>().AddAsync(userRole);
            await context.SaveChangesAsync();
        }
    }
}
