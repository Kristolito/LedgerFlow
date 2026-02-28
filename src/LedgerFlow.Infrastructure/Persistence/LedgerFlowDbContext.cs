using Microsoft.EntityFrameworkCore;

namespace LedgerFlow.Infrastructure.Persistence;

public sealed class LedgerFlowDbContext(DbContextOptions<LedgerFlowDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LedgerFlowDbContext).Assembly);
    }
}
