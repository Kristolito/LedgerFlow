using Stripe;

namespace LedgerFlow.Application.Abstractions.Billing;

public interface IStripeWebhookProcessor
{
    Task ProcessAsync(Event stripeEvent, CancellationToken cancellationToken);
}