using LedgerFlow.Api.Contracts.Billing;
using LedgerFlow.Api.Tenancy;
using LedgerFlow.Application.Billing.Commands.CreatePlan;
using LedgerFlow.Application.Billing.Commands.DeactivatePlan;
using LedgerFlow.Application.Billing.Queries.GetPlans;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LedgerFlow.Api.Controllers;

[ApiController]
[Route("plans")]
[Authorize]
[RequireTenant]
public sealed class PlansController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreatePlanRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreatePlanCommand(request.Name, request.PriceCents, request.Currency, request.Interval),
            cancellationToken);

        return CreatedAtAction(nameof(List), new { }, result);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetPlansQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeactivatePlanCommand(id), cancellationToken);
        return NoContent();
    }
}
