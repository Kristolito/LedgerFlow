using LedgerFlow.Application.Abstractions.Persistence;
using LedgerFlow.Application.Abstractions.Billing;
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
    IUserRepository userRepository,
    IStripeBillingService stripeBillingService,
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

        if (string.IsNullOrWhiteSpace(plan.StripePriceId))
        {
            throw new PlanStripePriceMissingException();
        }

        var userId = currentUserContext.UserId.Value;
        var user = await userRepository.GetByIdAsync(userId, cancellationToken)
                   ?? throw new UnauthorizedException("Authenticated user could not be found.");

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

        if (string.IsNullOrWhiteSpace(customer.StripeCustomerId))
        {
            var stripeCustomerId = await stripeBillingService.EnsureCustomerAsync(user.Email, cancellationToken);
            customer.StripeCustomerId = stripeCustomerId;
            await customerRepository.SaveChangesAsync(cancellationToken);
        }

        var stripeSubscription = await stripeBillingService.CreateSubscriptionAsync(
            customer.StripeCustomerId!,
            plan.StripePriceId,
            request.TrialDays,
            cancellationToken);

        var now = DateTimeOffset.UtcNow;
        var status = MapStripeStatus(stripeSubscription.status);
        DateTimeOffset? trialEndsAt = request.TrialDays > 0 ? now.AddDays(request.TrialDays) : null;

        var subscription = new Subscription
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            PlanId = plan.Id,
            Status = status,
            TrialEndsAt = trialEndsAt,
            CurrentPeriodStart = stripeSubscription.periodStart,
            CurrentPeriodEnd = stripeSubscription.periodEnd,
            CancelAtPeriodEnd = false,
            StripeSubscriptionId = stripeSubscription.subscriptionId,
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
            created.StripeSubscriptionId,
            created.CreatedAt);
    }

    private static SubscriptionStatus MapStripeStatus(string stripeStatus)
    {
        return stripeStatus.ToLowerInvariant() switch
        {
            "trialing" => SubscriptionStatus.Trialing,
            "active" => SubscriptionStatus.Active,
            "past_due" => SubscriptionStatus.PastDue,
            "canceled" => SubscriptionStatus.Canceled,
            "unpaid" => SubscriptionStatus.PastDue,
            "incomplete" => SubscriptionStatus.PastDue,
            "incomplete_expired" => SubscriptionStatus.Canceled,
            _ => SubscriptionStatus.Active
        };
    }
}
