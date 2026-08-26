using ComplianceSystem.Application.Authentication.Models;
using ComplianceSystem.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ComplianceSystem.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(
        UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<AuthenticatedUser?> AuthenticateAsync(
        string email,
        string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return null;
        }

        var isValidPassword =
            await _userManager.CheckPasswordAsync(user, password);

        if (!isValidPassword)
        {
            return null;
        }

        var roles = await _userManager.GetRolesAsync(user);

        return new AuthenticatedUser(
            user.Id.ToString(),
            user.Email!,
            roles.ToArray());
    }
}
