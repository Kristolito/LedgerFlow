using LedgerFlow.Application.Tenants.Dtos;
using MediatR;

namespace LedgerFlow.Application.Tenants.Queries.GetTenantSettings;

public sealed record GetTenantSettingsQuery : IRequest<IReadOnlyList<TenantSettingDto>>;
