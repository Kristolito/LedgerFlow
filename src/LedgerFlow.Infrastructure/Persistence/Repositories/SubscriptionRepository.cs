using LedgerFlow.Application.Abstractions.Persistence;
using LedgerFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LedgerFlow.Infrastructure.Persistence.Repositories;

public sealed class SubscriptionRepository(LedgerFlowDbContext dbContext) : ISubscriptionRepository
{
    public async Task<Subscription> CreateAsync(Subscription subscription, CancellationToken cancellationToken)
    {
        dbContext.Subscriptions.Add(subscription);
        await dbContext.SaveChangesAsync(cancellationToken);
        return subscription;
    }

    public async Task<IReadOnlyList<Subscription>> ListAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Subscriptions
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<Subscription?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Subscriptions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
