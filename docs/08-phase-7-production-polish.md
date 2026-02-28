# Phase 7 --- Production Polish

## Logging

-   Serilog structured logs
-   Enrich logs with:
    -   TenantId
    -   UserId
    -   CorrelationId

## Add

-   Health checks (DB, Redis)
-   Rate limiting on auth endpoints
-   ProblemDetails error responses
-   Swagger with JWT support

## Add README

Include: - Architecture overview - Docker setup instructions - Stripe
CLI webhook testing - Example curl commands

## Acceptance

-   Logs enriched properly
-   Swagger supports JWT
-   Docker compose runs entire stack
