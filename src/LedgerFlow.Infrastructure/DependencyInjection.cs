using LedgerFlow.Application.Abstractions.Persistence;
using LedgerFlow.Application.Abstractions.Tenancy;
using LedgerFlow.Infrastructure.Persistence;
using LedgerFlow.Infrastructure.Persistence.Repositories;
using LedgerFlow.Infrastructure.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LedgerFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'ConnectionStrings:Postgres' is not configured.");
        }

        services.AddScoped<ITenantContext, TenantContext>();
        services.AddDbContext<LedgerFlowDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<ITenantSettingRepository, TenantSettingRepository>();

        return services;
    }
}
