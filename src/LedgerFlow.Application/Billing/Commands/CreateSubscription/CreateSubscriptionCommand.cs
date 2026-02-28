using LedgerFlow.Application.Billing.Dtos;
using MediatR;

namespace LedgerFlow.Application.Billing.Commands.CreateSubscription;

public sealed record CreateSubscriptionCommand(Guid PlanId, int TrialDays) : IRequest<SubscriptionDto>;
