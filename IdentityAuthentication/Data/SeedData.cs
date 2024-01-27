using IdentityAuthentication.Model;
using Microsoft.AspNetCore.Identity;

namespace IdentityAuthentication.Data
{
    public class SeedData
    {

        private static async Task CreateRoleIfNotExists(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
        public static async Task InitializeAsync(RoleManager<IdentityRole> roleManager)
        {
            await CreateRoleIfNotExists(roleManager, UserRoles.Customer);
            await CreateRoleIfNotExists(roleManager, UserRoles.Admin);

        }
    }
}
