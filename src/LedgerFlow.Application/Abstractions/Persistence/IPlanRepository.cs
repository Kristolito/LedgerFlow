using LedgerFlow.Domain.Entities;

namespace LedgerFlow.Application.Abstractions.Persistence;

public interface IPlanRepository
{
    Task<Plan> CreateAsync(Plan plan, CancellationToken cancellationToken);
    Task<IReadOnlyList<Plan>> ListAsync(CancellationToken cancellationToken);
    Task<Plan?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
