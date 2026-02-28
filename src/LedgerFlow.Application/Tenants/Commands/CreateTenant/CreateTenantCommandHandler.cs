using LedgerFlow.Application.Abstractions.Persistence;
using LedgerFlow.Application.Common.Exceptions;
using LedgerFlow.Application.Tenants.Dtos;
using LedgerFlow.Domain.Entities;
using MediatR;

namespace LedgerFlow.Application.Tenants.Commands.CreateTenant;

public sealed class CreateTenantCommandHandler(ITenantRepository tenantRepository)
    : IRequestHandler<CreateTenantCommand, TenantDto>
{
    public async Task<TenantDto> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        var normalizedSlug = request.Slug.Trim().ToLowerInvariant();

        var slugExists = await tenantRepository.SlugExistsAsync(normalizedSlug, cancellationToken);
        if (slugExists)
        {
            throw new DuplicateSlugException(normalizedSlug);
        }

        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Slug = normalizedSlug,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var createdTenant = await tenantRepository.CreateAsync(tenant, cancellationToken);
        return new TenantDto(createdTenant.Id, createdTenant.Name, createdTenant.Slug, createdTenant.CreatedAt);
    }
}
