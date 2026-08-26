using ComplianceSystem.Application.Authentication.Models;

namespace ComplianceSystem.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<AuthenticatedUser?> AuthenticateAsync(
        string email,
        string password);
}