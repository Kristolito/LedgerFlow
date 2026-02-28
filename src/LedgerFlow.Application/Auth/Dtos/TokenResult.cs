namespace LedgerFlow.Application.Auth.Dtos;

public sealed record TokenResult(string AccessToken, DateTimeOffset ExpiresAtUtc);
