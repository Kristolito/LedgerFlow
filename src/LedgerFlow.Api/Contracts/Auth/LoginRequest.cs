namespace LedgerFlow.Api.Contracts.Auth;

public sealed record LoginRequest(string Email, string Password, Guid TenantId);
