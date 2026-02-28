using FluentValidation;

namespace LedgerFlow.Application.Auth.Commands.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    private const string SlugPattern = "^[a-z0-9-]{3,50}$";

    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(320);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(200);

        RuleFor(x => x.TenantName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.TenantSlug)
            .NotEmpty()
            .Matches(SlugPattern)
            .WithMessage("TenantSlug must be 3-50 chars and contain only lowercase letters, numbers, and hyphens.");
    }
}
