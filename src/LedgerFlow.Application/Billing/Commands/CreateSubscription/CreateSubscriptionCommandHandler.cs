using LedgerFlow.Application.Abstractions.Persistence;
using LedgerFlow.Application.Abstractions.Security;
using LedgerFlow.Application.Abstractions.Tenancy;
using LedgerFlow.Application.Billing.Dtos;
using LedgerFlow.Application.Common.Exceptions;
using LedgerFlow.Domain.Entities;
using LedgerFlow.Domain.Enums;
using MediatR;

namespace LedgerFlow.Application.Billing.Commands.CreateSubscription;

public sealed class CreateSubscriptionCommandHandler(
    IPlanRepository planRepository,
    ICustomerRepository customerRepository,
    ISubscriptionRepository subscriptionRepository,
    ICurrentUserContext currentUserContext,
    ITenantContext tenantContext) : IRequestHandler<CreateSubscriptionCommand, SubscriptionDto>
{
    public async Task<SubscriptionDto> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        if (!tenantContext.TenantId.HasValue)
        {
            throw new TenantContextMissingException("Tenant is required to create a subscription.");
        }

        if (!currentUserContext.UserId.HasValue)
        {
            throw new UnauthorizedException("Authenticated user context is required.");
        }

        var plan = await planRepository.GetByIdAsync(request.PlanId, cancellationToken);
        if (plan is null)
        {
            throw new NotFoundException("Plan not found.");
        }

        if (!plan.IsActive)
        {
            throw new BusinessRuleViolationException("Only active plans can be subscribed to.");
        }

        var userId = currentUserContext.UserId.Value;
        var customer = await customerRepository.GetByUserIdAsync(userId, cancellationToken);
        if (customer is null)
        {
            customer = await customerRepository.CreateAsync(new Customer
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CreatedAt = DateTimeOffset.UtcNow
            }, cancellationToken);
        }

        var now = DateTimeOffset.UtcNow;
        var periodEnd = plan.Interval == PlanInterval.Month
            ? now.AddMonths(1)
            : now.AddYears(1);

        var status = request.TrialDays > 0 ? SubscriptionStatus.Trialing : SubscriptionStatus.Active;
        DateTimeOffset? trialEndsAt = request.TrialDays > 0 ? now.AddDays(request.TrialDays) : null;

        var subscription = new Subscription
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            PlanId = plan.Id,
            Status = status,
            TrialEndsAt = trialEndsAt,
            CurrentPeriodStart = now,
            CurrentPeriodEnd = periodEnd,
            CancelAtPeriodEnd = false,
            CreatedAt = now
        };

        var created = await subscriptionRepository.CreateAsync(subscription, cancellationToken);
        return new SubscriptionDto(
            created.Id,
            created.CustomerId,
            created.PlanId,
            created.Status.ToString(),
            created.TrialEndsAt,
            created.CurrentPeriodStart,
            created.CurrentPeriodEnd,
            created.CancelAtPeriodEnd,
            created.CreatedAt);
    }
}
