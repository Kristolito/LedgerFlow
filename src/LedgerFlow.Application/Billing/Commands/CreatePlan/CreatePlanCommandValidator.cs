using FluentValidation;

namespace LedgerFlow.Application.Billing.Commands.CreatePlan;

public sealed class CreatePlanCommandValidator : AbstractValidator<CreatePlanCommand>
{
    public CreatePlanCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(120);

        RuleFor(x => x.PriceCents)
            .GreaterThan(0);

        RuleFor(x => x.Currency)
            .Must(x => string.IsNullOrWhiteSpace(x) || x.Trim().Length == 3)
            .WithMessage("Currency must be a 3-letter ISO code.");

        RuleFor(x => x.Interval)
            .NotEmpty()
            .Must(x => x is "Month" or "Year")
            .WithMessage("Interval must be Month or Year.");
    }
}
