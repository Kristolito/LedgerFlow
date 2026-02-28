using LedgerFlow.Application.Abstractions.Persistence;
using LedgerFlow.Application.Common.Exceptions;
using MediatR;

namespace LedgerFlow.Application.Billing.Commands.CancelSubscription;

public sealed class CancelSubscriptionCommandHandler(ISubscriptionRepository subscriptionRepository)
    : IRequestHandler<CancelSubscriptionCommand>
{
    public async Task Handle(CancelSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var subscription = await subscriptionRepository.GetByIdAsync(request.SubscriptionId, cancellationToken);
        if (subscription is null)
        {
            throw new NotFoundException("Subscription not found.");
        }

        subscription.CancelAtPeriodEnd = true;
        await subscriptionRepository.SaveChangesAsync(cancellationToken);
    }
}
