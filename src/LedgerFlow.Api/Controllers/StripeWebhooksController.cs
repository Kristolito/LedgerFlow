using System.Text;
using LedgerFlow.Api.Middleware;
using LedgerFlow.Application.Abstractions.Billing;
using LedgerFlow.Application.Abstractions.Persistence;
using LedgerFlow.Domain.Entities;
using LedgerFlow.Domain.Enums;
using LedgerFlow.Infrastructure.Billing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;

namespace LedgerFlow.Api.Controllers;

[ApiController]
[Route("webhooks/stripe")]
[AllowAnonymous]
public sealed class StripeWebhooksController(
    IStripeWebhookEventParser stripeWebhookEventParser,
    IStripeWebhookProcessor stripeWebhookProcessor,
    IWebhookEventRepository webhookEventRepository,
    IOptions<StripeOptions> stripeOptions,
    ILogger<StripeWebhooksController> logger) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Handle(CancellationToken cancellationToken)
    {
        var signatureHeader = Request.Headers["Stripe-Signature"].ToString();
        if (string.IsNullOrWhiteSpace(signatureHeader))
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid Stripe Signature",
                Detail = "Missing Stripe-Signature header."
            });
        }

        var webhookSecret = stripeOptions.Value.WebhookSecret;
        if (string.IsNullOrWhiteSpace(webhookSecret))
        {
            logger.LogError("Stripe webhook secret is not configured.");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Webhook Configuration Error",
                Detail = "Stripe webhook secret is not configured."
            });
        }

        string payloadJson;
        using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
        {
            payloadJson = await reader.ReadToEndAsync(cancellationToken);
        }

        Event stripeEvent;

        try
        {
            stripeEvent = stripeWebhookEventParser.ParseAndVerify(payloadJson, signatureHeader, webhookSecret);
        }
        catch (StripeException ex)
        {
            logger.LogWarning(ex, "Stripe webhook signature verification failed. CorrelationId={CorrelationId}",
                HttpContext.Response.Headers[CorrelationIdMiddleware.HeaderName].ToString());

            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid Stripe Signature",
                Detail = "Webhook signature verification failed."
            });
        }

        if (string.IsNullOrWhiteSpace(stripeEvent.Id))
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid Stripe Event",
                Detail = "Stripe event ID is missing."
            });
        }

        var correlationId = HttpContext.Response.Headers[CorrelationIdMiddleware.HeaderName].ToString();
        var eventType = stripeEvent.Type ?? string.Empty;

        using var scope = logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["StripeEventId"] = stripeEvent.Id,
            ["StripeEventType"] = eventType
        });

        var received = await webhookEventRepository.TryCreateReceivedAsync(new WebhookEvent
        {
            Id = Guid.NewGuid(),
            StripeEventId = stripeEvent.Id,
            Type = eventType,
            ReceivedAt = DateTimeOffset.UtcNow,
            Status = WebhookEventStatus.Received,
            PayloadJson = payloadJson
        }, cancellationToken);

        if (!received)
        {
            logger.LogInformation("Duplicate Stripe webhook event received. Returning 200 without processing.");
            return Ok();
        }

        try
        {
            await stripeWebhookProcessor.ProcessAsync(stripeEvent, cancellationToken);
            await webhookEventRepository.MarkProcessedAsync(stripeEvent.Id, cancellationToken);
            logger.LogInformation("Stripe webhook event processed successfully.");
            return Ok();
        }
        catch (Exception ex)
        {
            await webhookEventRepository.MarkFailedAsync(stripeEvent.Id, ex.Message, cancellationToken);
            logger.LogError(ex, "Stripe webhook event processing failed.");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Webhook Processing Error",
                Detail = "Failed to process Stripe webhook event."
            });
        }
    }
}