using LedgerFlow.Domain.Interfaces;

namespace LedgerFlow.Domain.Entities;

public sealed class TenantSetting : ITenantScoped
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}
