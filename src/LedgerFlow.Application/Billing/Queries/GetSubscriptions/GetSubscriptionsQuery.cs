using LedgerFlow.Application.Billing.Dtos;
using MediatR;

namespace LedgerFlow.Application.Billing.Queries.GetSubscriptions;

public sealed record GetSubscriptionsQuery : IRequest<IReadOnlyList<SubscriptionDto>>;
