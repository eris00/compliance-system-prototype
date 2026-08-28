using ComplianceSystem.Application.Cases.Commands.CreateCase;
using ComplianceSystem.Application.Cases.Commands.ResolveCase;
using ComplianceSystem.Application.Cases.Commands.StartReview;
using ComplianceSystem.Application.Common.Exceptions;
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

    [Authorize(Roles = AppRoles.Analyst)]
    [HttpPost("{id:guid}/start-review")]
    public async Task<IActionResult> StartReview(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            await _sender.Send(
                new StartReviewCommand(id),
                cancellationToken);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }

        return NoContent();
    }

    [Authorize(Roles = AppRoles.Analyst)]
    [HttpPost("{id:guid}/resolve")]
    public async Task<IActionResult> Resolve(
        Guid id,
        ResolveCaseRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _sender.Send(
                new ResolveCaseCommand(
                    id,
                    request.Outcome,
                    request.Explanation),
                cancellationToken);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }

        return NoContent();
    }

    [Authorize]
    [HttpGet("protected")]
    public IActionResult GetAll()
    {
        return Ok("Cases endpoint works.");
    }

    public sealed record ResolveCaseRequest(
        string Outcome,
        string Explanation);
}
