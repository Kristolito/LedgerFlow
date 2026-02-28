using LedgerFlow.Domain.Entities;

namespace LedgerFlow.Application.Abstractions.Persistence;

public interface ISubscriptionRepository
{
    Task<Subscription> CreateAsync(Subscription subscription, CancellationToken cancellationToken);
    Task<IReadOnlyList<Subscription>> ListAsync(CancellationToken cancellationToken);
    Task<Subscription?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
