using ComplianceSystem.Application.Authentication.Commands.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ComplianceSystem.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        var isValid = await _sender.Send(
            command,
            cancellationToken);

        if (!isValid)
        {
            return Unauthorized();
        }

        return Ok();
    }
}