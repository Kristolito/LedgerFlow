using LedgerFlow.Domain.Entities;

namespace LedgerFlow.Application.Abstractions.Persistence;

public interface IInvoiceRepository
{
    Task<IReadOnlyList<Invoice>> ListBySubscriptionIdAsync(Guid subscriptionId, CancellationToken cancellationToken);
}
