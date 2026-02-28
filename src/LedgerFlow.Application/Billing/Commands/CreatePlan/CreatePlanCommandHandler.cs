using LedgerFlow.Application.Abstractions.Persistence;
using LedgerFlow.Application.Billing.Dtos;
using LedgerFlow.Application.Common.Exceptions;
using LedgerFlow.Domain.Entities;
using LedgerFlow.Domain.Enums;
using MediatR;

namespace LedgerFlow.Application.Billing.Commands.CreatePlan;

public sealed class CreatePlanCommandHandler(IPlanRepository planRepository)
    : IRequestHandler<CreatePlanCommand, PlanDto>
{
    public async Task<PlanDto> Handle(CreatePlanCommand request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<PlanInterval>(request.Interval, true, out var interval))
        {
            throw new BusinessRuleViolationException("Invalid interval. Allowed values are Month and Year.");
        }

        var plan = new Plan
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            PriceCents = request.PriceCents,
            Currency = string.IsNullOrWhiteSpace(request.Currency)
                ? "GBP"
                : request.Currency.Trim().ToUpperInvariant(),
            Interval = interval,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var created = await planRepository.CreateAsync(plan, cancellationToken);

        return new PlanDto(
            created.Id,
            created.Name,
            created.PriceCents,
            created.Currency,
            created.Interval.ToString(),
            created.IsActive,
            created.CreatedAt);
    }
}
