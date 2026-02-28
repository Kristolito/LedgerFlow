using LedgerFlow.Api.Contracts.Tenants;
using LedgerFlow.Application.Tenants.Commands.CreateTenant;
using LedgerFlow.Application.Tenants.Queries.GetTenantById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LedgerFlow.Api.Controllers;

[ApiController]
[Route("tenants")]
public sealed class TenantsController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateTenant([FromBody] CreateTenantRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateTenantCommand(request.Name, request.Slug), cancellationToken);
        return CreatedAtAction(nameof(GetTenantById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTenantById(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetTenantByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }
}
