using ComplianceSystem.Application.Authentication.Commands.Login;
using ComplianceSystem.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ComplianceSystem.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUserService;

    public AuthController(
        ISender sender,
        ICurrentUserService currentUserService)
    {
        _sender = sender;
        _currentUserService = currentUserService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResult>> Login(
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            command,
            cancellationToken);

        if (result is null)
        {
            return Unauthorized();
        }

        return Ok(result);
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult<CurrentUserResponse> Me()
    {
        if (_currentUserService.UserId is not { } userId)
        {
            return Unauthorized();
        }

        return Ok(new CurrentUserResponse(
            userId,
            _currentUserService.Email,
            _currentUserService.Roles,
            _currentUserService.IsAuthenticated));
    }

    public record CurrentUserResponse(
        Guid UserId,
        string? Email,
        IReadOnlyCollection<string> Roles,
        bool IsAuthenticated);
}
