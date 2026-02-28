namespace LedgerFlow.Api.Contracts.Billing;

public sealed record CreateSubscriptionRequest(Guid PlanId, int TrialDays);
