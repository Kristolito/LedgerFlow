using LedgerFlow.Domain.Entities;

namespace LedgerFlow.Application.Abstractions.Persistence;

public interface ITenantMemberRepository
{
    Task<TenantMember> CreateAsync(TenantMember member, CancellationToken cancellationToken);
    Task<TenantMember?> GetByUserAndTenantAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken);
}
