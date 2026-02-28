using LedgerFlow.Application.Auth.Dtos;
using LedgerFlow.Domain.Enums;

namespace LedgerFlow.Application.Abstractions.Security;

public interface IJwtTokenGenerator
{
    TokenResult Generate(Guid userId, Guid tenantId, TenantRole role);
}
