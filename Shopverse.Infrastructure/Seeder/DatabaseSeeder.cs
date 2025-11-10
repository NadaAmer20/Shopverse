using Microsoft.EntityFrameworkCore;
using Shopverse.Domain.Entities;
using System.Threading.Tasks;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(DbContext context)
    {
        var roles = new List<Role>
    {
        new Role { Name = "Administrator", Key = "ADMINISTRATOR", Description = "Administrator Role" },
        new Role { Name = "Client", Key = "CLIENT", Description = "Client Role" },
        new Role { Name = "Seller", Key = "SELLER", Description = "Seller Role" },
        new Role { Name = "User", Key = "USER", Description = "Standard User Role" }
    };

       foreach (var role in roles)
        {
            var existingRole = await context.Set<Role>().FirstOrDefaultAsync(r => r.Name == role.Name);
            if (existingRole == null)
            {
                await context.Set<Role>().AddAsync(role);
                Console.WriteLine($"Role {role.Name} added.");
            }
        }

        await context.SaveChangesAsync();

        var allRoles = await context.Set<Role>().ToListAsync();

        var adminRole = await context.Set<Role>().FirstOrDefaultAsync(r => r.Name == "Administrator");
        var clientRole = await context.Set<Role>().FirstOrDefaultAsync(r => r.Name == "Client");
        var sellerRole = await context.Set<Role>().FirstOrDefaultAsync(r => r.Name == "Seller");
        var userRole = await context.Set<Role>().FirstOrDefaultAsync(r => r.Name == "User");

        if (adminRole == null || clientRole == null || sellerRole == null || userRole == null)
        {
            throw new Exception("Roles not found in the database.");
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
        }

        var adminUser = await context.Set<User>().FirstOrDefaultAsync(u => u.Email == "admin@pnu.local");
       if (adminUser == null)
        {
            adminUser = new User
            {
                Username = "admin",
                Email = "admin@pnu.local",
                IsActive = true,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
      //          RoleId = adminRole.Id  
            };
            await context.Set<User>().AddAsync(adminUser);
        }
 
        await context.SaveChangesAsync();
        Console.WriteLine("Users added successfully.");
    }

}
