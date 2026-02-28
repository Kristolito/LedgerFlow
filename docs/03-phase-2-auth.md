# Phase 2 --- Authentication & Tenant Membership

## Entities

User: - Id - Email - PasswordHash - CreatedAt

TenantMember: - TenantId - UserId - Role (Owner/Admin/Member)

## Implementation

-   JWT authentication
-   Password hashing (ASP.NET PasswordHasher)
-   JWT claims:
    -   sub (UserId)
    -   tid (TenantId)
    -   role

## Endpoints

-   POST /auth/register
-   POST /auth/login

## Authorization Policies

-   Owner
-   Admin

## Acceptance

-   Register returns JWT
-   Login returns JWT
-   Protected endpoints require auth
-   Role policies enforced
