using LedgerFlow.Application.Billing.Dtos;
using MediatR;

namespace LedgerFlow.Application.Billing.Commands.CreatePlan;

public sealed record CreatePlanCommand(
    string Name,
    int PriceCents,
    string Currency,
    string Interval) : IRequest<PlanDto>;
