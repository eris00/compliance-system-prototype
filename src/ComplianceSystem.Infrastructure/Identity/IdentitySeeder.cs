using Microsoft.AspNetCore.Identity;

namespace ComplianceSystem.Infrastructure.Identity;

public static class IdentitySeeder
{
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager)
    {
        const string email = "admin@compliance.local";
        const string password = "Admin123!";

        var existingUser = await userManager.FindByEmailAsync(email);

        if (existingUser is not null)
            return;

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                ", ",
                result.Errors.Select(e => e.Description));

            throw new Exception(
                $"Failed to seed Identity user: {errors}");
        }
    }
}