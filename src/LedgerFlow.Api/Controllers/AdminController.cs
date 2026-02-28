using LedgerFlow.Api.Tenancy;
using LedgerFlow.Application.Abstractions.Tenancy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LedgerFlow.Api.Controllers;

[ApiController]
[Route("admin")]
[Authorize(Policy = "AdminOrOwner")]
[RequireTenant]
public sealed class AdminController(ITenantContext tenantContext) : ControllerBase
{
    [HttpGet("ping")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Ping()
    {
        return Ok(new
        {
            Message = "pong",
            TenantId = tenantContext.TenantId
        });
    }
}
