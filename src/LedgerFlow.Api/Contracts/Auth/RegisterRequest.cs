namespace LedgerFlow.Api.Contracts.Auth;

public sealed record RegisterRequest(
    string Email,
    string Password,
    string TenantName,
    string TenantSlug);
