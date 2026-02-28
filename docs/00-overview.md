# Multi-Tenant SaaS Billing Platform

## Goal

Build a production-style Multi-Tenant SaaS Subscription & Billing
Platform using:

-   .NET 8
-   Clean Architecture
-   PostgreSQL
-   Stripe (test mode)
-   Hangfire
-   Docker
-   JWT Authentication
-   Serilog logging

This project demonstrates:

-   System design
-   Multi-tenancy
-   Payment processing
-   Webhook idempotency
-   Background job processing
-   Production-ready patterns

## Architecture

Projects:

-   Billing.Api
-   Billing.Application
-   Billing.Domain
-   Billing.Infrastructure
-   Billing.Worker

Architecture Style: Modular Monolith using Clean Architecture
principles.
