namespace LedgerFlow.Application.Auth.Dtos;

public sealed record AuthResponseDto(
    string AccessToken,
    DateTimeOffset ExpiresAtUtc,
    Guid UserId,
    Guid TenantId,
    string Role);
