using ComplianceSystem.Application.Cases.Commands.CloseCase;
using ComplianceSystem.Application.Cases.Commands.CreateCase;
using ComplianceSystem.Application.Cases.Commands.ResolveCase;
using ComplianceSystem.Application.Cases.Commands.StartReview;
using ComplianceSystem.Application.Cases.Dtos;
using ComplianceSystem.Application.Cases.Queries.GetCaseAuditTrail;
using ComplianceSystem.Application.Cases.Queries.GetCaseDetails;
using ComplianceSystem.Application.Cases.Queries.GetCases;
using ComplianceSystem.Application.Common.Exceptions;
using ComplianceSystem.Application.Common.Security;
using ComplianceSystem.Domain.Enums;
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

    [Authorize(Roles = AppRoles.Analyst + "," + AppRoles.Supervisor + "," + AppRoles.Auditor)]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CaseListItemDto>>> GetCases(
        [FromQuery] CaseStatus? status,
        [FromQuery] SeverityLevel? severity,
        [FromQuery] Guid? categoryId,
        [FromQuery] bool? isEscalated,
        [FromQuery] Guid? assignedAnalystId,
        CancellationToken cancellationToken)
    {
        if (status is { } statusValue
            && !Enum.IsDefined(statusValue))
        {
            return BadRequest(CreateInvalidEnumProblemDetails(
                nameof(status),
                "Case status filter is invalid."));
        }

        if (severity is { } severityValue
            && !Enum.IsDefined(severityValue))
        {
            return BadRequest(CreateInvalidEnumProblemDetails(
                nameof(severity),
                "Case severity filter is invalid."));
        }

        var result = await _sender.Send(
            new GetCasesQuery(
                status,
                severity,
                categoryId,
                isEscalated,
                assignedAnalystId),
            cancellationToken);

        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Analyst + "," + AppRoles.Supervisor + "," + AppRoles.Auditor)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CaseDetailsDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _sender.Send(
                new GetCaseDetailsQuery(id),
                cancellationToken);

            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [Authorize(Roles = AppRoles.Analyst + "," + AppRoles.Supervisor + "," + AppRoles.Auditor)]
    [HttpGet("{id:guid}/audit")]
    public async Task<ActionResult<IReadOnlyList<AuditEntryDto>>> GetAuditTrail(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _sender.Send(
                new GetCaseAuditTrailQuery(id),
                cancellationToken);

            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
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

    [Authorize(Roles = AppRoles.Supervisor)]
    [HttpPost("{id:guid}/close")]
    public async Task<IActionResult> Close(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            await _sender.Send(
                new CloseCaseCommand(id),
                cancellationToken);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }

        return NoContent();
    }

    public sealed record ResolveCaseRequest(
        string Outcome,
        string Explanation);

    private static ProblemDetails CreateInvalidEnumProblemDetails(
        string parameterName,
        string detail)
    {
        return new ProblemDetails
        {
            Title = "Invalid query parameter.",
            Detail = $"{detail} Parameter: {parameterName}.",
            Status = StatusCodes.Status400BadRequest
        };
    }
}
