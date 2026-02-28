using LedgerFlow.Application.Abstractions.Persistence;
using LedgerFlow.Application.Billing.Dtos;
using MediatR;

namespace LedgerFlow.Application.Billing.Queries.GetPlans;

public sealed class GetPlansQueryHandler(IPlanRepository planRepository)
    : IRequestHandler<GetPlansQuery, IReadOnlyList<PlanDto>>
{
    public async Task<IReadOnlyList<PlanDto>> Handle(GetPlansQuery request, CancellationToken cancellationToken)
    {
        var plans = await planRepository.ListAsync(cancellationToken);
        return plans
            .Select(x => new PlanDto(
                x.Id,
                x.Name,
                x.PriceCents,
                x.Currency,
                x.Interval.ToString(),
                x.IsActive,
                x.CreatedAt))
            .ToList();
    }
}
