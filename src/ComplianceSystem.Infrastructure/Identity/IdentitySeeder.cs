using ComplianceSystem.Application.Common.Security;
using Microsoft.AspNetCore.Identity;

namespace ComplianceSystem.Infrastructure.Identity;

public static class IdentitySeeder
{
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        foreach (var role in AppRoles.All)
        {
            if (await roleManager.RoleExistsAsync(role))
            {
                continue;
            }

            var roleResult = await roleManager.CreateAsync(
                new IdentityRole<Guid>(role));

            if (!roleResult.Succeeded)
            {
                ThrowSeedException(
                    $"Failed to seed Identity role '{role}'",
                    roleResult.Errors);
            }
        }

        var demoUsers = new[]
        {
            new DemoUser("analyst1@compliance.local", AppRoles.Analyst),
            new DemoUser("analyst2@compliance.local", AppRoles.Analyst),
            new DemoUser("supervisor@compliance.local", AppRoles.Supervisor),
            new DemoUser("auditor@compliance.local", AppRoles.Auditor)
        };

        foreach (var demoUser in demoUsers)
        {
            await SeedDemoUserAsync(userManager, demoUser);
        }
    }

    private static async Task SeedDemoUserAsync(
        UserManager<ApplicationUser> userManager,
        DemoUser demoUser)
    {
        const string password = "Admin123!";

        var user = await userManager.FindByEmailAsync(demoUser.Email);

        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = demoUser.Email,
                Email = demoUser.Email,
                EmailConfirmed = true
            };

            var createUserResult = await userManager.CreateAsync(user, password);

            if (!createUserResult.Succeeded)
            {
                ThrowSeedException(
                    $"Failed to seed Identity user '{demoUser.Email}'",
                    createUserResult.Errors);
            }
        }

        if (await userManager.IsInRoleAsync(user, demoUser.Role))
        {
            return;
        }

        var addToRoleResult = await userManager.AddToRoleAsync(
            user,
            demoUser.Role);

        if (!addToRoleResult.Succeeded)
        {
            ThrowSeedException(
                $"Failed to assign role '{demoUser.Role}' to '{demoUser.Email}'",
                addToRoleResult.Errors);
        }
    }

    private static void ThrowSeedException(
        string message,
        IEnumerable<IdentityError> errors)
    {
        var errorText = string.Join(
            ", ",
            errors.Select(e => e.Description));

        throw new Exception($"{message}: {errorText}");
    }

    private record DemoUser(
        string Email,
        string Role);
}
