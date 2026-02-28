using LedgerFlow.Application.Abstractions.Persistence;
using LedgerFlow.Application.Abstractions.Security;
using LedgerFlow.Application.Auth.Dtos;
using LedgerFlow.Application.Common.Exceptions;
using LedgerFlow.Domain.Entities;
using LedgerFlow.Domain.Enums;
using MediatR;

namespace LedgerFlow.Application.Auth.Commands.Register;

public sealed class RegisterCommandHandler(
    IUserRepository userRepository,
    ITenantRepository tenantRepository,
    ITenantMemberRepository tenantMemberRepository,
    IPasswordHasherService passwordHasherService,
    IJwtTokenGenerator jwtTokenGenerator) : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var normalizedSlug = request.TenantSlug.Trim().ToLowerInvariant();

        if (await userRepository.EmailExistsAsync(normalizedEmail, cancellationToken))
        {
            throw new DuplicateEmailException(normalizedEmail);
        }

        if (await tenantRepository.SlugExistsAsync(normalizedSlug, cancellationToken))
        {
            throw new DuplicateSlugException(normalizedSlug);
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = normalizedEmail,
            CreatedAt = DateTimeOffset.UtcNow
        };
        user.PasswordHash = passwordHasherService.HashPassword(user, request.Password);
        user = await userRepository.CreateAsync(user, cancellationToken);

        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = request.TenantName.Trim(),
            Slug = normalizedSlug,
            CreatedAt = DateTimeOffset.UtcNow
        };
        tenant = await tenantRepository.CreateAsync(tenant, cancellationToken);

        var member = new TenantMember
        {
            TenantId = tenant.Id,
            UserId = user.Id,
            Role = TenantRole.Owner,
            CreatedAt = DateTimeOffset.UtcNow
        };
        member = await tenantMemberRepository.CreateAsync(member, cancellationToken);

        var token = jwtTokenGenerator.Generate(user.Id, tenant.Id, member.Role);
        return new AuthResponseDto(token.AccessToken, token.ExpiresAtUtc, user.Id, tenant.Id, member.Role.ToString());
    }
}
