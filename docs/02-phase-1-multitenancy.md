# Phase 1 --- Multi-Tenancy

## Goal

Implement row-level multi-tenancy.

## Entities

Tenant: - Id (Guid) - Name - Slug - CreatedAt

## Implementation

-   Add ITenantScoped interface with TenantId
-   Add TenantResolutionMiddleware
-   Read TenantId from:
    -   JWT claim (preferred)
    -   X-Tenant-Id header (fallback)
-   Store TenantId in ITenantContext (scoped service)
-   Add EF Core global query filter for tenant-scoped entities
-   Automatically assign TenantId in SaveChanges

## Endpoints

-   POST /tenants
-   GET /tenants/{id}

## Acceptance

-   Tenant created successfully
-   Data isolation enforced
-   Cross-tenant reads not possible
