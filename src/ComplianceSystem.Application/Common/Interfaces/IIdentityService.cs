using ComplianceSystem.Application.Authentication.Models;

namespace ComplianceSystem.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<AuthenticatedUser?> AuthenticateAsync(
        string email,
        string password);

    Task<bool> UserExistsAsync(Guid userId);

    Task<bool> IsInRoleAsync(
        Guid userId,
        string role);

    Task<IReadOnlyDictionary<Guid, string>> GetUserNamesAsync(
        IEnumerable<Guid> userIds,
        CancellationToken cancellationToken);
}
