using LedgerFlow.Application.Abstractions.Persistence;
using LedgerFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LedgerFlow.Infrastructure.Persistence.Repositories;

public sealed class PlanRepository(LedgerFlowDbContext dbContext) : IPlanRepository
{
    public async Task<Plan> CreateAsync(Plan plan, CancellationToken cancellationToken)
    {
        dbContext.Plans.Add(plan);
        await dbContext.SaveChangesAsync(cancellationToken);
        return plan;
    }

    public async Task<IReadOnlyList<Plan>> ListAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Plans
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<Plan?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Plans.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
