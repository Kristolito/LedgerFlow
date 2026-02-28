using LedgerFlow.Application.Abstractions.Persistence;
using LedgerFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LedgerFlow.Infrastructure.Persistence.Repositories;

public sealed class TenantSettingRepository(LedgerFlowDbContext dbContext) : ITenantSettingRepository
{
    public async Task<TenantSetting> CreateAsync(TenantSetting tenantSetting, CancellationToken cancellationToken)
    {
        dbContext.TenantSettings.Add(tenantSetting);
        await dbContext.SaveChangesAsync(cancellationToken);
        return tenantSetting;
    }

    public async Task<IReadOnlyList<TenantSetting>> GetAllForCurrentTenantAsync(CancellationToken cancellationToken)
    {
        return await dbContext.TenantSettings
            .AsNoTracking()
            .OrderBy(x => x.Key)
            .ToListAsync(cancellationToken);
    }
}
