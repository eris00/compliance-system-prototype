using ComplianceSystem.Application.Cases.Commands.CreateCase;
using ComplianceSystem.Application.Common.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComplianceSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CasesController : ControllerBase
{
    private readonly ISender _sender;

    public CasesController(ISender sender)
    {
        _sender = sender;
    }

    [Authorize(Roles = AppRoles.Analyst + "," + AppRoles.Supervisor)]
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateCaseCommand command,
        CancellationToken cancellationToken)
    {
        var caseId = await _sender.Send(
            command,
            cancellationToken);

        return Created(
            $"/api/cases/{caseId}",
            new { id = caseId });
    }

    [Authorize]
    [HttpGet("protected")]
    public IActionResult GetAll()
    {
        return Ok("Cases endpoint works.");
    }
}
