using LedgerFlow.Application.Abstractions.Persistence;
using LedgerFlow.Application.Abstractions.Security;
using LedgerFlow.Application.Auth.Dtos;
using LedgerFlow.Application.Common.Exceptions;
using MediatR;

namespace LedgerFlow.Application.Auth.Commands.Login;

public sealed class LoginCommandHandler(
    IUserRepository userRepository,
    ITenantMemberRepository tenantMemberRepository,
    IPasswordHasherService passwordHasherService,
    IJwtTokenGenerator jwtTokenGenerator) : IRequestHandler<LoginCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (user is null || !passwordHasherService.VerifyPassword(user, user.PasswordHash, request.Password))
        {
            throw new InvalidCredentialsException();
        }

        var membership = await tenantMemberRepository.GetByUserAndTenantAsync(user.Id, request.TenantId, cancellationToken);
        if (membership is null)
        {
            throw new ForbiddenException("User is not a member of the requested tenant.");
        }

        var token = jwtTokenGenerator.Generate(user.Id, membership.TenantId, membership.Role);
        return new AuthResponseDto(
            token.AccessToken,
            token.ExpiresAtUtc,
            user.Id,
            membership.TenantId,
            membership.Role.ToString());
    }
}
