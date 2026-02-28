namespace LedgerFlow.Application.Tenants.Dtos;

public sealed record TenantSettingDto(Guid Id, Guid TenantId, string Key, string Value, DateTimeOffset CreatedAt);
