using LedgerFlow.Application.Auth.Dtos;
using MediatR;

namespace LedgerFlow.Application.Auth.Commands.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string TenantName,
    string TenantSlug) : IRequest<AuthResponseDto>;
