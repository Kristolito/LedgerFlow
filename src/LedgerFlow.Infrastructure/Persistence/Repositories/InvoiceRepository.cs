using LedgerFlow.Application.Abstractions.Persistence;
using LedgerFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LedgerFlow.Infrastructure.Persistence.Repositories;

public sealed class InvoiceRepository(LedgerFlowDbContext dbContext) : IInvoiceRepository
{
    public async Task<IReadOnlyList<Invoice>> ListBySubscriptionIdAsync(Guid subscriptionId, CancellationToken cancellationToken)
    {
        return await dbContext.Invoices
            .AsNoTracking()
            .Where(x => x.SubscriptionId == subscriptionId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
