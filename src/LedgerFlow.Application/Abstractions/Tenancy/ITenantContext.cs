namespace LedgerFlow.Application.Abstractions.Tenancy;

public interface ITenantContext
{
    Guid? TenantId { get; }
    bool HasTenant { get; }
    void SetTenantId(Guid tenantId);
}
