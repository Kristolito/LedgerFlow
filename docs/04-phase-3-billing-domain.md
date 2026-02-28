# Phase 3 --- Billing Domain

## Entities

Plan: - Id - Name - PriceCents - Interval (Month/Year) - StripePriceId -
IsActive

Customer: - Id - TenantId - UserId - StripeCustomerId

Subscription: - Id - TenantId - CustomerId - PlanId - Status -
TrialEndsAt - CurrentPeriodStart - CurrentPeriodEnd -
CancelAtPeriodEnd - StripeSubscriptionId - CreatedAt

Invoice: - Id - TenantId - SubscriptionId - AmountCents - Currency -
Status - StripeInvoiceId - CreatedAt

## Endpoints

-   POST /plans
-   GET /plans
-   POST /subscriptions
-   POST /subscriptions/{id}/cancel

## Acceptance

-   Plans can be created
-   Subscription created in DB
-   Cancel marks CancelAtPeriodEnd
