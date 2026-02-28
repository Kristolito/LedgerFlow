using LedgerFlow.Domain.Interfaces;

namespace LedgerFlow.Domain.Entities;

public sealed class Customer : ITenantScoped
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public string? StripeCustomerId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
