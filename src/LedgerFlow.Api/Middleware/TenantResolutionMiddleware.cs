using System.Security.Claims;
using LedgerFlow.Api.Tenancy;
using LedgerFlow.Application.Abstractions.Tenancy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LedgerFlow.Api.Middleware;

public sealed class TenantResolutionMiddleware(RequestDelegate next)
{
    public const string TenantSourceItemKey = "TenantResolutionSource";

    public async Task Invoke(HttpContext context, ITenantContext tenantContext)
    {
        var endpoint = context.GetEndpoint();
        var requiresTenant = endpoint?.Metadata.GetMetadata<IRequireTenantMetadata>() is not null;
        var requiresAuthorization = endpoint?.Metadata.GetMetadata<IAuthorizeData>() is not null;

        var (tenantId, source, invalidReason) = ResolveTenant(context);
        context.Items[TenantSourceItemKey] = source;

        if (invalidReason is not null)
        {
            await WriteProblemAsync(
                context,
                StatusCodes.Status400BadRequest,
                "Invalid Tenant Identifier",
                invalidReason);
            return;
        }

        if (tenantId.HasValue)
        {
            tenantContext.SetTenantId(tenantId.Value);
        }

        if (requiresTenant && !tenantContext.HasTenant)
        {
            if (requiresAuthorization && context.User.Identity?.IsAuthenticated != true)
            {
                await next(context);
                return;
            }

            await WriteProblemAsync(
                context,
                StatusCodes.Status400BadRequest,
                "Tenant Required",
                "A tenant context is required for this endpoint. Provide JWT claim 'tid' or header 'X-Tenant-Id'.");
            return;
        }

        await next(context);
    }

    private static (Guid? tenantId, string source, string? invalidReason) ResolveTenant(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated is true)
        {
            var tidClaim = context.User.FindFirstValue("tid");
            if (!string.IsNullOrWhiteSpace(tidClaim))
            {
                if (Guid.TryParse(tidClaim, out var tenantId))
                {
                    return (tenantId, "jwt", null);
                }

                return (null, "jwt", "JWT claim 'tid' is not a valid GUID.");
            }
        }

        var headerValue = context.Request.Headers["X-Tenant-Id"].ToString();
        if (!string.IsNullOrWhiteSpace(headerValue))
        {
            if (Guid.TryParse(headerValue, out var tenantId))
            {
                return (tenantId, "header", null);
            }

            return (null, "header", "Header 'X-Tenant-Id' is not a valid GUID.");
        }

        return (null, "none", null);
    }

    private static Task WriteProblemAsync(HttpContext context, int status, string title, string detail)
    {
        context.Response.StatusCode = status;
        return context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail
        });
    }
}
