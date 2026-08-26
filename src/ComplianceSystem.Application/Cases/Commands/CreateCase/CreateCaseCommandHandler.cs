using ComplianceSystem.Application.Common.Interfaces;
using MediatR;

namespace ComplianceSystem.Application.Authentication.Commands.Login;

public class LoginCommandHandler
    : IRequestHandler<LoginCommand, bool>
{
    private readonly IIdentityService _identityService;

    public LoginCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<bool> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        return await _identityService.CheckPasswordAsync(
            request.Email,
            request.Password);
    }
}