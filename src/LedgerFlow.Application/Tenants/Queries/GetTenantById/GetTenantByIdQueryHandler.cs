using LedgerFlow.Application.Abstractions.Persistence;
using LedgerFlow.Application.Tenants.Dtos;
using MediatR;

namespace LedgerFlow.Application.Tenants.Queries.GetTenantById;

public sealed class GetTenantByIdQueryHandler(ITenantRepository tenantRepository)
    : IRequestHandler<GetTenantByIdQuery, TenantDto?>
{
    public async Task<TenantDto?> Handle(GetTenantByIdQuery request, CancellationToken cancellationToken)
    {
        var tenant = await tenantRepository.GetByIdAsync(request.TenantId, cancellationToken);
        return tenant is null
            ? null
            : new TenantDto(tenant.Id, tenant.Name, tenant.Slug, tenant.CreatedAt);
    }
}
