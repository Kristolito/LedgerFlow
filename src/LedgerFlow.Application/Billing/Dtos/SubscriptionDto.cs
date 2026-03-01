namespace LedgerFlow.Application.Billing.Dtos;

public sealed record SubscriptionDto(
    Guid Id,
    Guid CustomerId,
    Guid PlanId,
    string Status,
    DateTimeOffset? TrialEndsAt,
    DateTimeOffset CurrentPeriodStart,
    DateTimeOffset CurrentPeriodEnd,
    bool CancelAtPeriodEnd,
    string? StripeSubscriptionId,
    DateTimeOffset CreatedAt);
