# Phase 5 --- Stripe Webhooks & Idempotency

## Endpoint

POST /webhooks/stripe

## Implementation

-   Verify Stripe signature
-   Store events in WebhookEvent table:
    -   StripeEventId (unique)
    -   Type
    -   PayloadJson
    -   Status
-   If StripeEventId exists → return 200 (idempotent)

## Handle Events

-   invoice.paid
-   invoice.payment_failed
-   customer.subscription.updated
-   customer.subscription.deleted

## Acceptance

-   Duplicate events processed only once
-   Subscription statuses update correctly
