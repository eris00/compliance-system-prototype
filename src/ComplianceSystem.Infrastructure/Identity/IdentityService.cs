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

    public async Task<bool> CheckPasswordAsync(
        string email,
        string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return false;

        return await _userManager.CheckPasswordAsync(user, password);
    }
}