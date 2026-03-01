using LedgerFlow.Application.Abstractions.Billing;
using LedgerFlow.Application.Common.Exceptions;
using Microsoft.Extensions.Options;
using Stripe;

namespace LedgerFlow.Infrastructure.Billing;

public sealed class StripeBillingService(IOptions<StripeOptions> options) : IStripeBillingService
{
    private readonly StripeOptions _stripeOptions = options.Value;

    public async Task<string> EnsureCustomerAsync(string email, CancellationToken ct)
    {
        EnsureConfigured();
        StripeConfiguration.ApiKey = _stripeOptions.SecretKey;

        try
        {
            var customerService = new CustomerService();
            var customer = await customerService.CreateAsync(
                new CustomerCreateOptions
                {
                    Email = email
                },
                cancellationToken: ct);

            return customer.Id;
        }
        catch (StripeException ex)
        {
            throw new StripeIntegrationException("Stripe customer creation failed.", ex);
        }
    }

    public async Task<(string subscriptionId, string status, DateTimeOffset periodStart, DateTimeOffset periodEnd)>
        CreateSubscriptionAsync(
            string stripeCustomerId,
            string stripePriceId,
            int trialDays,
            CancellationToken ct)
    {
        EnsureConfigured();
        StripeConfiguration.ApiKey = _stripeOptions.SecretKey;

        try
        {
            var subscriptionService = new SubscriptionService();
            var createOptions = new SubscriptionCreateOptions
            {
                Customer = stripeCustomerId,
                Items =
                [
                    new SubscriptionItemOptions
                    {
                        Price = stripePriceId
                    }
                ]
            };

            if (trialDays > 0)
            {
                createOptions.TrialPeriodDays = trialDays;
            }

            var subscription = await subscriptionService.CreateAsync(createOptions, cancellationToken: ct);
            return (
                subscription.Id,
                subscription.Status,
                new DateTimeOffset(DateTime.SpecifyKind(subscription.CurrentPeriodStart, DateTimeKind.Utc)),
                new DateTimeOffset(DateTime.SpecifyKind(subscription.CurrentPeriodEnd, DateTimeKind.Utc)));
        }
        catch (StripeException ex)
        {
            throw new StripeIntegrationException("Stripe subscription creation failed.", ex);
        }
    }

    public async Task<string> CreateOrGetPriceAsync(
        string currency,
        int unitAmountCents,
        string interval,
        string productName,
        CancellationToken ct)
    {
        EnsureConfigured();
        StripeConfiguration.ApiKey = _stripeOptions.SecretKey;

        try
        {
            var productService = new ProductService();
            var product = await productService.CreateAsync(
                new ProductCreateOptions
                {
                    Name = productName
                },
                cancellationToken: ct);

            var priceService = new PriceService();
            var price = await priceService.CreateAsync(
                new PriceCreateOptions
                {
                    Currency = currency.ToLowerInvariant(),
                    UnitAmount = unitAmountCents,
                    Product = product.Id,
                    Recurring = new PriceRecurringOptions
                    {
                        Interval = interval.ToLowerInvariant() switch
                        {
                            "month" => "month",
                            "year" => "year",
                            _ => throw new StripeIntegrationException("Unsupported billing interval for Stripe price.")
                        }
                    }
                },
                cancellationToken: ct);

            return price.Id;
        }
        catch (StripeException ex)
        {
            throw new StripeIntegrationException("Stripe price sync failed.", ex);
        }
    }

    private void EnsureConfigured()
    {
        if (string.IsNullOrWhiteSpace(_stripeOptions.SecretKey))
        {
            throw new StripeIntegrationException("Stripe secret key is not configured.");
        }
    }
}
