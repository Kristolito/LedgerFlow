using LedgerFlow.Application.Auth.Dtos;
using MediatR;

namespace LedgerFlow.Application.Auth.Commands.Login;

public sealed record LoginCommand(
    string Email,
    string Password,
    Guid TenantId) : IRequest<AuthResponseDto>;
