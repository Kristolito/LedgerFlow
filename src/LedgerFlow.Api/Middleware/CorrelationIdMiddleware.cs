using Serilog.Context;

namespace LedgerFlow.Api.Middleware;

public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    public const string HeaderName = "X-Correlation-Id";

    public async Task Invoke(HttpContext context)
    {
        var incomingCorrelationId = context.Request.Headers[HeaderName].ToString();

        var correlationId = !string.IsNullOrWhiteSpace(incomingCorrelationId)
            ? incomingCorrelationId.Trim()
            : Guid.NewGuid().ToString();

        context.Response.Headers[HeaderName] = correlationId;

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await next(context);
        }
    }
}
