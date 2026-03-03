using Stripe;

namespace LedgerFlow.Application.Abstractions.Billing;

public interface IStripeWebhookEventParser
{
    Event ParseAndVerify(string payloadJson, string signatureHeader, string webhookSecret);
}