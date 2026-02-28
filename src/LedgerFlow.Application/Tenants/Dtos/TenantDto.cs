namespace LedgerFlow.Application.Tenants.Dtos;

public sealed record TenantDto(Guid Id, string Name, string Slug, DateTimeOffset CreatedAt);
