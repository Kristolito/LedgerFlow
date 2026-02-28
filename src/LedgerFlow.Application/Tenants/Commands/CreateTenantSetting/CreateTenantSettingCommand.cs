using LedgerFlow.Application.Tenants.Dtos;
using MediatR;

namespace LedgerFlow.Application.Tenants.Commands.CreateTenantSetting;

public sealed record CreateTenantSettingCommand(string Key, string Value) : IRequest<TenantSettingDto>;
