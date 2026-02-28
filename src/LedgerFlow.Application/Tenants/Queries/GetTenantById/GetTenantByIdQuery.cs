using LedgerFlow.Application.Tenants.Dtos;
using MediatR;

namespace LedgerFlow.Application.Tenants.Queries.GetTenantById;

public sealed record GetTenantByIdQuery(Guid TenantId) : IRequest<TenantDto?>;
