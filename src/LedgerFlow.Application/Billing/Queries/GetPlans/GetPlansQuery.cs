using LedgerFlow.Application.Billing.Dtos;
using MediatR;

namespace LedgerFlow.Application.Billing.Queries.GetPlans;

public sealed record GetPlansQuery : IRequest<IReadOnlyList<PlanDto>>;
