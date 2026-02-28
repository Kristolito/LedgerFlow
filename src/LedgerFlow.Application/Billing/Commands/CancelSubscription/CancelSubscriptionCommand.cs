using MediatR;

namespace LedgerFlow.Application.Billing.Commands.CancelSubscription;

public sealed record CancelSubscriptionCommand(Guid SubscriptionId) : IRequest;
