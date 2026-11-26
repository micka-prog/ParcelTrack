using Microsoft.AspNetCore.Identity;

namespace ParcelTrack.Data
{
    // Runs each time the app starts (Program.cs calls it).
    // 1. Makes sure the Admin and Staff roles exist.
    // 2. Makes the two demo accounts named in appsettings.json if they are missing,
    //    and gives each its role. Both use DemoPassword.
    //    This is a practice app: a real app never keeps a password in appsettings.json.
    public static class RoleSeed
    {
        public static async Task Run(IServiceProvider services, IConfiguration config)
        {
            RoleManager<IdentityRole> role_manager = services.GetRequiredService<RoleManager<IdentityRole>>();
            UserManager<IdentityUser> user_manager = services.GetRequiredService<UserManager<IdentityUser>>();

            string[] role_names = { "Admin", "Staff" };
            foreach (string role_name in role_names)
            {
                bool role_exists = await role_manager.RoleExistsAsync(role_name);
                if (!role_exists)
                {
                    await role_manager.CreateAsync(new IdentityRole(role_name));
                }
            }

            string? password = config["DemoPassword"];
            await MakeAccount(user_manager, config["AdminEmail"], password, "Admin");
            await MakeAccount(user_manager, config["StaffEmail"], password, "Staff");
        }

        private static async Task MakeAccount(UserManager<IdentityUser> user_manager, string? email, string? password, string role)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return;
            }

            IdentityUser? user = await user_manager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new IdentityUser();
                user.UserName = email;
                user.Email = email;
                user.EmailConfirmed = true;
                await user_manager.CreateAsync(user, password);
            }

            bool has_role = await user_manager.IsInRoleAsync(user, role);
            if (!has_role)
            {
                await user_manager.AddToRoleAsync(user, role);
            }
        }
    }
}
