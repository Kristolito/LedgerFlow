using LedgerFlow.Application.Abstractions.Persistence;
using LedgerFlow.Application.Common.Exceptions;
using LedgerFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace LedgerFlow.Infrastructure.Persistence.Repositories;

public sealed class TenantRepository(LedgerFlowDbContext dbContext) : ITenantRepository
{
    public async Task<Tenant> CreateAsync(Tenant tenant, CancellationToken cancellationToken)
    {
        dbContext.Tenants.Add(tenant);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            throw new DuplicateSlugException(tenant.Slug);
        }

        return tenant;
    }

    public Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken)
    {
        return dbContext.Tenants
            .AsNoTracking()
            .AnyAsync(x => x.Slug == slug, cancellationToken);
    }

    private static bool IsUniqueViolation(DbUpdateException exception)
    {
        return exception.InnerException is PostgresException postgresException &&
               postgresException.SqlState == PostgresErrorCodes.UniqueViolation;
    }
}
