using LedgerFlow.Application.Abstractions.Persistence;
using LedgerFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LedgerFlow.Infrastructure.Persistence.Repositories;

public sealed class CustomerRepository(LedgerFlowDbContext dbContext) : ICustomerRepository
{
    public Task<Customer?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return dbContext.Customers
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
    }

    public async Task<Customer> CreateAsync(Customer customer, CancellationToken cancellationToken)
    {
        dbContext.Customers.Add(customer);
        await dbContext.SaveChangesAsync(cancellationToken);
        return customer;
    }
}
