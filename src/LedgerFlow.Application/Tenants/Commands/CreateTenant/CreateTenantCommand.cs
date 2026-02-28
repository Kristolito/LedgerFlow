using LedgerFlow.Application.Tenants.Dtos;
using MediatR;

namespace LedgerFlow.Application.Tenants.Commands.CreateTenant;

public sealed record CreateTenantCommand(string Name, string Slug) : IRequest<TenantDto>;
