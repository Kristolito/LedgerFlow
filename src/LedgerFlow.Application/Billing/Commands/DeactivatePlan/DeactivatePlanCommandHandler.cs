using LedgerFlow.Application.Abstractions.Persistence;
using LedgerFlow.Application.Common.Exceptions;
using MediatR;

namespace LedgerFlow.Application.Billing.Commands.DeactivatePlan;

public sealed class DeactivatePlanCommandHandler(IPlanRepository planRepository) : IRequestHandler<DeactivatePlanCommand>
{
    public async Task Handle(DeactivatePlanCommand request, CancellationToken cancellationToken)
    {
        var plan = await planRepository.GetByIdAsync(request.PlanId, cancellationToken);
        if (plan is null)
        {
            throw new NotFoundException("Plan not found.");
        }

        plan.IsActive = false;
        await planRepository.SaveChangesAsync(cancellationToken);
    }
}
