namespace ComplianceSystem.Application.Common.Interfaces;

public interface ITokenService
{
    string CreateToken(
        string userId,
        string email);
}