using Microsoft.AspNetCore.Mvc;

namespace ComplianceSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CasesController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok("Cases endpoint works.");
    }
}