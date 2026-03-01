using LedgerFlow.Application.Abstractions.Persistence;
using LedgerFlow.Application.Billing.Dtos;
using MediatR;

namespace LedgerFlow.Application.Billing.Queries.GetSubscriptions;

public sealed class GetSubscriptionsQueryHandler(ISubscriptionRepository subscriptionRepository)
    : IRequestHandler<GetSubscriptionsQuery, IReadOnlyList<SubscriptionDto>>
{
    public async Task<IReadOnlyList<SubscriptionDto>> Handle(
        GetSubscriptionsQuery request,
        CancellationToken cancellationToken)
    {
        var subscriptions = await subscriptionRepository.ListAsync(cancellationToken);
        return subscriptions
            .Select(x => new SubscriptionDto(
                x.Id,
                x.CustomerId,
                x.PlanId,
                x.Status.ToString(),
                x.TrialEndsAt,
                x.CurrentPeriodStart,
                x.CurrentPeriodEnd,
                x.CancelAtPeriodEnd,
                x.StripeSubscriptionId,
                x.CreatedAt))
            .ToList();
    }
}
