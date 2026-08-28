using ComplianceSystem.Application.Common.Security;
using ComplianceSystem.Application.Dashboard.Queries.GetDashboardSummary;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComplianceSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = AppRoles.Analyst + "," + AppRoles.Supervisor + "," + AppRoles.Auditor)]
public class DashboardController : ControllerBase
{
    private readonly ISender _sender;

    public DashboardController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<DashboardSummaryDto>> Get(
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetDashboardSummaryQuery(),
            cancellationToken);

        return Ok(result);
    }
}
