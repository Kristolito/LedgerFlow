using LedgerFlow.Api.Tenancy;
using LedgerFlow.Application.Billing.Commands.SyncStripePrices;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LedgerFlow.Api.Controllers;

[ApiController]
[Route("stripe")]
[Authorize(Policy = "AdminOrOwner")]
[RequireTenant]
public sealed class StripeController(ISender sender) : ControllerBase
{
    [HttpPost("prices/sync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SyncPrices(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new SyncStripePricesCommand(), cancellationToken);
        return Ok(result);
    }
}
