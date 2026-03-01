using LedgerFlow.Application.Abstractions.Billing;
using LedgerFlow.Application.Abstractions.Persistence;
using LedgerFlow.Application.Billing.Dtos;
using MediatR;

namespace LedgerFlow.Application.Billing.Commands.SyncStripePrices;

public sealed class SyncStripePricesCommandHandler(
    IPlanRepository planRepository,
    IStripeBillingService stripeBillingService) : IRequestHandler<SyncStripePricesCommand, StripePriceSyncResultDto>
{
    public async Task<StripePriceSyncResultDto> Handle(SyncStripePricesCommand request, CancellationToken cancellationToken)
    {
        var plans = await planRepository.ListAsync(cancellationToken);
        var activePlans = plans.Where(x => x.IsActive).ToList();
        var pendingPlans = activePlans.Where(x => string.IsNullOrWhiteSpace(x.StripePriceId)).ToList();
        var createdCount = 0;

        foreach (var plan in pendingPlans)
        {
            var trackedPlan = await planRepository.GetByIdAsync(plan.Id, cancellationToken);
            if (trackedPlan is null)
            {
                continue;
            }

            var stripePriceId = await stripeBillingService.CreateOrGetPriceAsync(
                trackedPlan.Currency,
                trackedPlan.PriceCents,
                trackedPlan.Interval.ToString(),
                trackedPlan.Name,
                cancellationToken);

            trackedPlan.StripePriceId = stripePriceId;
            createdCount++;
        }

        if (createdCount > 0)
        {
            await planRepository.SaveChangesAsync(cancellationToken);
        }

        var skippedCount = activePlans.Count - pendingPlans.Count;
        return new StripePriceSyncResultDto(createdCount, skippedCount);
    }
}
