using LedgerFlow.Application.Abstractions.Tenancy;

namespace LedgerFlow.Infrastructure.Tenancy;

public sealed class TenantContext : ITenantContext
{
    public Guid? TenantId { get; private set; }
    public bool HasTenant => TenantId.HasValue;

    public void SetTenantId(Guid tenantId)
    {
        TenantId = tenantId;
    }
}
