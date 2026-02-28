using LedgerFlow.Application.Abstractions.Persistence;
using LedgerFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LedgerFlow.Infrastructure.Persistence.Repositories;

public sealed class TenantMemberRepository(LedgerFlowDbContext dbContext) : ITenantMemberRepository
{
    public async Task<TenantMember> CreateAsync(TenantMember member, CancellationToken cancellationToken)
    {
        dbContext.TenantMembers.Add(member);
        await dbContext.SaveChangesAsync(cancellationToken);
        return member;
    }

    public Task<TenantMember?> GetByUserAndTenantAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken)
    {
        return dbContext.TenantMembers
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId && x.TenantId == tenantId, cancellationToken);
    }
}
