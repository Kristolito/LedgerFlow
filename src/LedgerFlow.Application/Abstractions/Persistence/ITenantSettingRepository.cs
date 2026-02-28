using LedgerFlow.Domain.Entities;

namespace LedgerFlow.Application.Abstractions.Persistence;

public interface ITenantSettingRepository
{
    Task<TenantSetting> CreateAsync(TenantSetting tenantSetting, CancellationToken cancellationToken);
    Task<IReadOnlyList<TenantSetting>> GetAllForCurrentTenantAsync(CancellationToken cancellationToken);
}
