using FluentValidation;

namespace LedgerFlow.Application.Billing.Commands.CreateSubscription;

public sealed class CreateSubscriptionCommandValidator : AbstractValidator<CreateSubscriptionCommand>
{
    public CreateSubscriptionCommandValidator()
    {
        RuleFor(x => x.PlanId)
            .NotEqual(Guid.Empty);

        RuleFor(x => x.TrialDays)
            .InclusiveBetween(0, 30);
    }
}
