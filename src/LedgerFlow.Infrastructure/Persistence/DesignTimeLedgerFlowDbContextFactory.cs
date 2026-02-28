using LedgerFlow.Application.Abstractions.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LedgerFlow.Infrastructure.Persistence;

public sealed class DesignTimeLedgerFlowDbContextFactory : IDesignTimeDbContextFactory<LedgerFlowDbContext>
{
    public LedgerFlowDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LedgerFlowDbContext>();
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Postgres") ??
                               "Host=localhost;Port=5432;Database=ledgerflow;Username=ledgerflow;Password=ledgerflow";

        optionsBuilder.UseNpgsql(connectionString);

        return new LedgerFlowDbContext(optionsBuilder.Options, new DesignTimeTenantContext());
    }

    private sealed class DesignTimeTenantContext : ITenantContext
    {
        public Guid? TenantId => null;
        public bool HasTenant => false;
        public void SetTenantId(Guid tenantId)
        {
        }
    }
}
