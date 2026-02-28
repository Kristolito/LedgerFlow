namespace LedgerFlow.Application.Billing.Dtos;

public sealed record InvoiceDto(
    Guid Id,
    Guid SubscriptionId,
    int AmountCents,
    string Currency,
    string Status,
    DateTimeOffset CreatedAt);
