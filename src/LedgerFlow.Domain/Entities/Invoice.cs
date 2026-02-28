using LedgerFlow.Domain.Enums;
using LedgerFlow.Domain.Interfaces;

namespace LedgerFlow.Domain.Entities;

public sealed class Invoice : ITenantScoped
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid SubscriptionId { get; set; }
    public int AmountCents { get; set; }
    public string Currency { get; set; } = "GBP";
    public InvoiceStatus Status { get; set; }
    public string? StripeInvoiceId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
