# Phase 6 --- Background Jobs

## Use Hangfire

-   Configure Postgres storage
-   Expose dashboard at /hangfire (admin only)

## Recurring Jobs

RetryFailedPaymentsJob: - Runs hourly - Retries PastDue subscriptions

SendReceiptEmailJob: - Log-only stub for now

## Acceptance

-   Hangfire dashboard accessible
-   Recurring jobs scheduled
-   Jobs execute successfully
