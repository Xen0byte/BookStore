using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace API.Data
{
    public static class SeedData
    {
        public static async Task Seed(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            await SeedRoles(roleManager);
            await SeedUsers(userManager);
        }

        private static async Task SeedUsers(UserManager<IdentityUser> userManager)
        {
            if (await userManager.FindByEmailAsync("admin@bookstore.com") is null)
            {
                var user = new IdentityUser
                {
                    UserName = "admin",
                    Email = "admin@bookstore.com"
                };

                var result = await userManager.CreateAsync(user, "P@ssword00");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Administrator");
                }
            }

            for (var i = 1; i <= 10; i++)
            {
                if (await userManager.FindByEmailAsync($"customer-{i:D2}@outlook.com") is not null) continue;

                var user = new IdentityUser
                {
                    UserName = $"customer-{i:D2}",
                    Email = $"customer-{i:D2}@outlook.com"
                };

                var result = await userManager.CreateAsync(user, $"P@ssword{i:D2}");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Customer");
                }
            }
        }

        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Administrator"))
            {
                var role = new IdentityRole("Administrator");
                await roleManager.CreateAsync(role);
            }

            if (!await roleManager.RoleExistsAsync("Customer"))
            {
                var role = new IdentityRole("Customer");
                await roleManager.CreateAsync(role);
            }
        }
    }
}