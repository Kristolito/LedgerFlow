using System.IdentityModel.Tokens.Jwt;
using LedgerFlow.Api.Tenancy;
using LedgerFlow.Application.Abstractions.Persistence;
using LedgerFlow.Application.Abstractions.Tenancy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LedgerFlow.Api.Controllers;

[ApiController]
public sealed class IdentityController(IUserRepository userRepository, ITenantContext tenantContext) : ControllerBase
{
    [HttpGet("me")]
    [Authorize]
    [RequireTenant]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!Guid.TryParse(sub, out var userId))
        {
            return Unauthorized();
        }

        var user = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return Unauthorized();
        }

        var role = User.FindFirst("role")?.Value ?? "Unknown";
        return Ok(new
        {
            UserId = user.Id,
            user.Email,
            TenantId = tenantContext.TenantId,
            Role = role
        });
    }
}
