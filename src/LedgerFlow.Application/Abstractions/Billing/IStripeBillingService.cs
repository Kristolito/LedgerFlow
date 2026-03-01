namespace LedgerFlow.Application.Abstractions.Billing;

public interface IStripeBillingService
{
    Task<string> EnsureCustomerAsync(string email, CancellationToken ct);

    Task<(string subscriptionId, string status, DateTimeOffset periodStart, DateTimeOffset periodEnd)>
        CreateSubscriptionAsync(
            string stripeCustomerId,
            string stripePriceId,
            int trialDays,
            CancellationToken ct);

    Task<string> CreateOrGetPriceAsync(
        string currency,
        int unitAmountCents,
        string interval,
        string productName,
        CancellationToken ct);
}
