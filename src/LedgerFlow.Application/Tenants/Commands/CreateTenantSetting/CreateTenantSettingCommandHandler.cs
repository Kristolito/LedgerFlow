using LedgerFlow.Application.Abstractions.Persistence;
using LedgerFlow.Application.Tenants.Dtos;
using LedgerFlow.Domain.Entities;
using MediatR;

namespace LedgerFlow.Application.Tenants.Commands.CreateTenantSetting;

public sealed class CreateTenantSettingCommandHandler(ITenantSettingRepository tenantSettingRepository)
    : IRequestHandler<CreateTenantSettingCommand, TenantSettingDto>
{
    public async Task<TenantSettingDto> Handle(CreateTenantSettingCommand request, CancellationToken cancellationToken)
    {
        var tenantSetting = new TenantSetting
        {
            Id = Guid.NewGuid(),
            Key = request.Key.Trim(),
            Value = request.Value.Trim(),
            CreatedAt = DateTimeOffset.UtcNow
        };

        var created = await tenantSettingRepository.CreateAsync(tenantSetting, cancellationToken);
        return new TenantSettingDto(created.Id, created.TenantId, created.Key, created.Value, created.CreatedAt);
    }
}
