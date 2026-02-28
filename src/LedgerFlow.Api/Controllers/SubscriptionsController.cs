using LedgerFlow.Api.Contracts.Billing;
using LedgerFlow.Api.Tenancy;
using LedgerFlow.Application.Billing.Commands.CancelSubscription;
using LedgerFlow.Application.Billing.Commands.CreateSubscription;
using LedgerFlow.Application.Billing.Queries.GetSubscriptionInvoices;
using LedgerFlow.Application.Billing.Queries.GetSubscriptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LedgerFlow.Api.Controllers;

[ApiController]
[Route("subscriptions")]
[Authorize]
[RequireTenant]
public sealed class SubscriptionsController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateSubscriptionRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateSubscriptionCommand(request.PlanId, request.TrialDays), cancellationToken);
        return CreatedAtAction(nameof(List), new { }, result);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetSubscriptionsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new CancelSubscriptionCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpGet("{id:guid}/invoices")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInvoices(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetSubscriptionInvoicesQuery(id), cancellationToken);
        return Ok(result);
    }
}
