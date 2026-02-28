using LedgerFlow.Domain.Entities;

namespace LedgerFlow.Application.Abstractions.Persistence;

public interface ICustomerRepository
{
    Task<Customer?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<Customer> CreateAsync(Customer customer, CancellationToken cancellationToken);
}
