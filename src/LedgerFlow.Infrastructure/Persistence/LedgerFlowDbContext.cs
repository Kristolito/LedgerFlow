using System.Linq.Expressions;
using LedgerFlow.Application.Abstractions.Tenancy;
using LedgerFlow.Application.Common.Exceptions;
using LedgerFlow.Domain.Entities;
using LedgerFlow.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LedgerFlow.Infrastructure.Persistence;

public sealed class LedgerFlowDbContext(
    DbContextOptions<LedgerFlowDbContext> options,
    ITenantContext tenantContext) : DbContext(options)
{
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<TenantSetting> TenantSettings => Set<TenantSetting>();
    public DbSet<User> Users => Set<User>();
    public DbSet<TenantMember> TenantMembers => Set<TenantMember>();
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<Invoice> Invoices => Set<Invoice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LedgerFlowDbContext).Assembly);
        ApplyTenantQueryFilters(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        EnforceTenantBoundaries();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        EnforceTenantBoundaries();
        return base.SaveChanges();
    }

    private void ApplyTenantQueryFilters(ModelBuilder modelBuilder)
    {
        var tenantScopedTypes = modelBuilder.Model.GetEntityTypes()
            .Where(t => t.ClrType is not null && typeof(ITenantScoped).IsAssignableFrom(t.ClrType))
            .Select(t => t.ClrType)
            .ToList();

        foreach (var clrType in tenantScopedTypes)
        {
            var method = typeof(LedgerFlowDbContext)
                .GetMethod(nameof(ApplyTenantQueryFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .MakeGenericMethod(clrType);

            method.Invoke(this, [modelBuilder]);
        }
    }

    private void ApplyTenantQueryFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : class, ITenantScoped
    {
        Expression<Func<TEntity, bool>> filterExpression =
            entity => tenantContext.HasTenant && entity.TenantId == tenantContext.TenantId;

        modelBuilder.Entity<TEntity>().HasQueryFilter(filterExpression);
    }

    private void EnforceTenantBoundaries()
    {
        var entries = ChangeTracker.Entries<ITenantScoped>()
            .Where(x => x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    HandleAdded(entry);
                    break;
                case EntityState.Modified:
                case EntityState.Deleted:
                    HandleModifiedOrDeleted(entry);
                    break;
            }
        }
    }

    private void HandleAdded(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<ITenantScoped> entry)
    {
        if (entry.Entity.TenantId == Guid.Empty)
        {
            if (!tenantContext.TenantId.HasValue)
            {
                throw new TenantContextMissingException(
                    "TenantId is required for tenant-scoped inserts but no tenant context was resolved.");
            }

            entry.Entity.TenantId = tenantContext.TenantId.Value;
            return;
        }

        if (tenantContext.HasTenant && entry.Entity.TenantId != tenantContext.TenantId)
        {
            throw new TenantIdMutationException(
                "Tenant-scoped insert TenantId must match the resolved tenant context.");
        }
    }

    private void HandleModifiedOrDeleted(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<ITenantScoped> entry)
    {
        if (entry.Property(nameof(ITenantScoped.TenantId)).IsModified)
        {
            throw new TenantIdMutationException("TenantId cannot be changed on existing tenant-scoped entities.");
        }

        if (!tenantContext.TenantId.HasValue)
        {
            throw new TenantContextMissingException(
                "TenantId is required for tenant-scoped updates/deletes but no tenant context was resolved.");
        }

        if (entry.Entity.TenantId != tenantContext.TenantId.Value)
        {
            throw new TenantIdMutationException("Resolved tenant does not match the entity TenantId.");
        }
    }
}
