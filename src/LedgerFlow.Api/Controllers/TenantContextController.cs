using LedgerFlow.Api.Middleware;
using LedgerFlow.Application.Abstractions.Tenancy;
using Microsoft.AspNetCore.Mvc;

namespace LedgerFlow.Api.Controllers;

[ApiController]
[Route("tenant-context")]
public sealed class TenantContextController(ITenantContext tenantContext) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        var source = HttpContext.Items.TryGetValue(TenantResolutionMiddleware.TenantSourceItemKey, out var value)
            ? value?.ToString() ?? "none"
            : "none";

        return Ok(new
        {
            TenantId = tenantContext.TenantId,
            Source = source
        });
    }
}
