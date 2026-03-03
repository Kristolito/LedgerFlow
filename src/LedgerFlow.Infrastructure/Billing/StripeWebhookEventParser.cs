using LedgerFlow.Application.Abstractions.Billing;
using Stripe;

namespace LedgerFlow.Infrastructure.Billing;

public sealed class StripeWebhookEventParser : IStripeWebhookEventParser
{
    public Event ParseAndVerify(string payloadJson, string signatureHeader, string webhookSecret)
    {
        return EventUtility.ConstructEvent(payloadJson, signatureHeader, webhookSecret);
    }
}