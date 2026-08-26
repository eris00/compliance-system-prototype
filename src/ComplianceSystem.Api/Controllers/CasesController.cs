using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComplianceSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CasesController : ControllerBase
{
    [Authorize]
    [HttpGet("protected")]
    public IActionResult GetAll()
    {
        return Ok("Cases endpoint works.");
    }
}