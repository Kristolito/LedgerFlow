using LedgerFlow.Application.Abstractions.Billing;
using LedgerFlow.Application.Abstractions.Tenancy;
using LedgerFlow.Domain.Entities;
using LedgerFlow.Domain.Enums;
using LedgerFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stripe;
using DomainInvoice = LedgerFlow.Domain.Entities.Invoice;
using DomainSubscription = LedgerFlow.Domain.Entities.Subscription;
using StripeInvoice = Stripe.Invoice;
using StripeSubscription = Stripe.Subscription;

namespace LedgerFlow.Infrastructure.Billing;

public sealed class StripeWebhookProcessor(
    LedgerFlowDbContext dbContext,
    ITenantContext tenantContext,
    ILogger<StripeWebhookProcessor> logger) : IStripeWebhookProcessor
{
    public async Task ProcessAsync(Event stripeEvent, CancellationToken cancellationToken)
    {
        switch (stripeEvent.Type)
        {
            case "invoice.paid":
                await HandleInvoicePaidAsync(stripeEvent, cancellationToken);
                break;
            case "invoice.payment_failed":
                await HandleInvoicePaymentFailedAsync(stripeEvent, cancellationToken);
                break;
            case "customer.subscription.updated":
                await HandleSubscriptionUpdatedAsync(stripeEvent, cancellationToken);
                break;
            case "customer.subscription.deleted":
                await HandleSubscriptionDeletedAsync(stripeEvent, cancellationToken);
                break;
            default:
                logger.LogInformation("Skipping unsupported Stripe webhook event type {StripeEventType}", stripeEvent.Type);
                break;
        }
    }

    private async Task HandleInvoicePaidAsync(Event stripeEvent, CancellationToken cancellationToken)
    {
        var stripeInvoice = stripeEvent.Data.Object as StripeInvoice;
        if (stripeInvoice is null)
        {
            logger.LogWarning("Stripe invoice.paid payload could not be parsed for event {StripeEventId}", stripeEvent.Id);
            return;
        }

        var subscription = await FindSubscriptionForInvoiceAsync(stripeInvoice, cancellationToken);
        if (subscription is null)
        {
            logger.LogWarning(
                "No subscription found while handling invoice.paid for Stripe invoice {StripeInvoiceId} and subscription {StripeSubscriptionId}",
                stripeInvoice.Id,
                stripeInvoice.SubscriptionId);
            return;
        }

        tenantContext.SetTenantId(subscription.TenantId);

        await UpsertInvoiceAsync(subscription, stripeInvoice, InvoiceStatus.Paid, cancellationToken);

        if (subscription.Status != SubscriptionStatus.Canceled)
        {
            subscription.Status = SubscriptionStatus.Active;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleInvoicePaymentFailedAsync(Event stripeEvent, CancellationToken cancellationToken)
    {
        var stripeInvoice = stripeEvent.Data.Object as StripeInvoice;
        if (stripeInvoice is null)
        {
            logger.LogWarning("Stripe invoice.payment_failed payload could not be parsed for event {StripeEventId}", stripeEvent.Id);
            return;
        }

        var subscription = await FindSubscriptionForInvoiceAsync(stripeInvoice, cancellationToken);
        if (subscription is null)
        {
            logger.LogWarning(
                "No subscription found while handling invoice.payment_failed for Stripe invoice {StripeInvoiceId} and subscription {StripeSubscriptionId}",
                stripeInvoice.Id,
                stripeInvoice.SubscriptionId);
            return;
        }

        tenantContext.SetTenantId(subscription.TenantId);

        await UpsertInvoiceAsync(subscription, stripeInvoice, InvoiceStatus.Failed, cancellationToken);
        subscription.Status = SubscriptionStatus.PastDue;

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleSubscriptionUpdatedAsync(Event stripeEvent, CancellationToken cancellationToken)
    {
        var stripeSubscription = stripeEvent.Data.Object as StripeSubscription;
        if (stripeSubscription is null)
        {
            logger.LogWarning("Stripe customer.subscription.updated payload could not be parsed for event {StripeEventId}", stripeEvent.Id);
            return;
        }

        var subscription = await dbContext.Subscriptions
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.StripeSubscriptionId == stripeSubscription.Id, cancellationToken);

        if (subscription is null)
        {
            logger.LogWarning(
                "No subscription found while handling customer.subscription.updated for Stripe subscription {StripeSubscriptionId}",
                stripeSubscription.Id);
            return;
        }

        tenantContext.SetTenantId(subscription.TenantId);

        subscription.Status = MapStripeStatus(stripeSubscription.Status);
        subscription.CurrentPeriodStart = stripeSubscription.CurrentPeriodStart;
        subscription.CurrentPeriodEnd = stripeSubscription.CurrentPeriodEnd;
        subscription.CancelAtPeriodEnd = stripeSubscription.CancelAtPeriodEnd;
        subscription.TrialEndsAt = stripeSubscription.TrialEnd;

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleSubscriptionDeletedAsync(Event stripeEvent, CancellationToken cancellationToken)
    {
        var stripeSubscription = stripeEvent.Data.Object as StripeSubscription;
        if (stripeSubscription is null)
        {
            logger.LogWarning("Stripe customer.subscription.deleted payload could not be parsed for event {StripeEventId}", stripeEvent.Id);
            return;
        }

        var subscription = await dbContext.Subscriptions
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.StripeSubscriptionId == stripeSubscription.Id, cancellationToken);

        if (subscription is null)
        {
            logger.LogWarning(
                "No subscription found while handling customer.subscription.deleted for Stripe subscription {StripeSubscriptionId}",
                stripeSubscription.Id);
            return;
        }

        tenantContext.SetTenantId(subscription.TenantId);
        subscription.Status = SubscriptionStatus.Canceled;

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<DomainSubscription?> FindSubscriptionForInvoiceAsync(StripeInvoice stripeInvoice, CancellationToken cancellationToken)
    {
        DomainSubscription? subscription = null;

        if (!string.IsNullOrWhiteSpace(stripeInvoice.SubscriptionId))
        {
            subscription = await dbContext.Subscriptions
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.StripeSubscriptionId == stripeInvoice.SubscriptionId, cancellationToken);
        }

        if (subscription is not null || string.IsNullOrWhiteSpace(stripeInvoice.Id))
        {
            return subscription;
        }

        var existingInvoice = await dbContext.Invoices
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.StripeInvoiceId == stripeInvoice.Id, cancellationToken);

        if (existingInvoice is null)
        {
            return null;
        }

        return await dbContext.Subscriptions
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == existingInvoice.SubscriptionId, cancellationToken);
    }

    private async Task UpsertInvoiceAsync(
        DomainSubscription subscription,
        StripeInvoice stripeInvoice,
        InvoiceStatus status,
        CancellationToken cancellationToken)
    {
        DomainInvoice? invoice = null;

        if (!string.IsNullOrWhiteSpace(stripeInvoice.Id))
        {
            invoice = await dbContext.Invoices
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.StripeInvoiceId == stripeInvoice.Id, cancellationToken);
        }

        var amountCents = stripeInvoice.AmountPaid > 0 ? stripeInvoice.AmountPaid : stripeInvoice.AmountDue;

        if (invoice is null)
        {
            invoice = new DomainInvoice
            {
                Id = Guid.NewGuid(),
                TenantId = subscription.TenantId,
                SubscriptionId = subscription.Id,
                AmountCents = (int)amountCents,
                Currency = (stripeInvoice.Currency ?? "GBP").ToUpperInvariant(),
                Status = status,
                StripeInvoiceId = stripeInvoice.Id,
                CreatedAt = DateTimeOffset.UtcNow
            };

            dbContext.Invoices.Add(invoice);
            return;
        }

        invoice.SubscriptionId = subscription.Id;
        invoice.AmountCents = (int)amountCents;
        invoice.Currency = (stripeInvoice.Currency ?? invoice.Currency).ToUpperInvariant();
        invoice.Status = status;
        invoice.TenantId = subscription.TenantId;
    }

    private static SubscriptionStatus MapStripeStatus(string? stripeStatus)
    {
        return stripeStatus?.ToLowerInvariant() switch
        {
            "trialing" => SubscriptionStatus.Trialing,
            "active" => SubscriptionStatus.Active,
            "past_due" => SubscriptionStatus.PastDue,
            "unpaid" => SubscriptionStatus.PastDue,
            "canceled" => SubscriptionStatus.Canceled,
            _ => SubscriptionStatus.Active
        };
    }
}
