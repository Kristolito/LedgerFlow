# Phase 0 --- Solution Setup

## Goal

Create solution structure and ensure project builds.

## Tasks

1.  Create .NET 8 solution.
2.  Add projects:
    -   Billing.Api
    -   Billing.Application
    -   Billing.Domain
    -   Billing.Infrastructure
    -   Billing.Worker
3.  Add project references: Api → Application + Infrastructure
    Application → Domain Infrastructure → Application + Domain Worker →
    Application + Infrastructure
4.  Add health check endpoint `/health`
5.  Add Serilog with console logging.
6.  Add correlation ID middleware.
7.  Add environment-based appsettings.

## Acceptance Criteria

-   dotnet build succeeds
-   GET /health returns 200 OK
