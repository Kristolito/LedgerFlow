namespace LedgerFlow.Application.Billing.Dtos;

public sealed record PlanDto(
    Guid Id,
    string Name,
    int PriceCents,
    string Currency,
    string Interval,
    bool IsActive,
    DateTimeOffset CreatedAt);
