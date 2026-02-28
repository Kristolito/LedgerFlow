using LedgerFlow.Application.Abstractions.Persistence;
using LedgerFlow.Application.Tenants.Dtos;
using MediatR;

namespace LedgerFlow.Application.Tenants.Queries.GetTenantSettings;

public sealed class GetTenantSettingsQueryHandler(ITenantSettingRepository tenantSettingRepository)
    : IRequestHandler<GetTenantSettingsQuery, IReadOnlyList<TenantSettingDto>>
{
    public async Task<IReadOnlyList<TenantSettingDto>> Handle(
        GetTenantSettingsQuery request,
        CancellationToken cancellationToken)
    {
        var settings = await tenantSettingRepository.GetAllForCurrentTenantAsync(cancellationToken);
        return settings
            .Select(x => new TenantSettingDto(x.Id, x.TenantId, x.Key, x.Value, x.CreatedAt))
            .ToList();
    }
}
