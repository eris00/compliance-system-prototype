using MediatR;

namespace ComplianceSystem.Application.Authentication.Commands.Login;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<LoginResult?>;