using LedgerFlow.Domain.Enums;
using LedgerFlow.Domain.Interfaces;

namespace LedgerFlow.Domain.Entities;

public sealed class TenantMember : ITenantScoped
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public TenantRole Role { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
