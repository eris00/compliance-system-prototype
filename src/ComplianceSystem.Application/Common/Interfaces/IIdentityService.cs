namespace ComplianceSystem.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<bool> CheckPasswordAsync(
        string email,
        string password);
}