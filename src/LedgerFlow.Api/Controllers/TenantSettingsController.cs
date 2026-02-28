using LedgerFlow.Api.Contracts.Tenants;
using LedgerFlow.Api.Tenancy;
using LedgerFlow.Application.Tenants.Commands.CreateTenantSetting;
using LedgerFlow.Application.Tenants.Queries.GetTenantSettings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LedgerFlow.Api.Controllers;

[ApiController]
[Route("tenant-settings")]
[RequireTenant]
public sealed class TenantSettingsController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateTenantSettingRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateTenantSettingCommand(request.Key, request.Value), cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { }, result);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetTenantSettingsQuery(), cancellationToken);
        return Ok(result);
    }
}
