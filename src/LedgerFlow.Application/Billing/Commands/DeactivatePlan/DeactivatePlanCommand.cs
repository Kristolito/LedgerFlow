using MediatR;

namespace LedgerFlow.Application.Billing.Commands.DeactivatePlan;

public sealed record DeactivatePlanCommand(Guid PlanId) : IRequest;
