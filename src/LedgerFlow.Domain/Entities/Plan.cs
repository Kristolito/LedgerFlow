using LedgerFlow.Domain.Enums;
using LedgerFlow.Domain.Interfaces;

namespace LedgerFlow.Domain.Entities;

public sealed class Plan : ITenantScoped
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int PriceCents { get; set; }
    public string Currency { get; set; } = "GBP";
    public PlanInterval Interval { get; set; }
    public string? StripePriceId { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
