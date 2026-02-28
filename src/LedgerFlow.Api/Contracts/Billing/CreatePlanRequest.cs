namespace LedgerFlow.Api.Contracts.Billing;

public sealed record CreatePlanRequest(string Name, int PriceCents, string Currency, string Interval);
