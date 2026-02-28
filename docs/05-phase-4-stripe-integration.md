# Phase 4 --- Stripe Integration

## Requirements

-   Use Stripe test mode
-   Secret key from environment variables
-   Webhook secret configured

## On Subscription Creation

1.  Create Stripe Customer (if not exists)
2.  Create Stripe Subscription
3.  Store:
    -   StripeCustomerId
    -   StripeSubscriptionId

## Admin Endpoint

-   POST /stripe/prices/sync

## Acceptance

-   Stripe customer created
-   Stripe subscription created
-   IDs stored in DB
