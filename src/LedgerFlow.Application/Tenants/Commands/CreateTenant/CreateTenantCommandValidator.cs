using FluentValidation;

namespace LedgerFlow.Application.Tenants.Commands.CreateTenant;

public sealed class CreateTenantCommandValidator : AbstractValidator<CreateTenantCommand>
{
    private const string SlugPattern = "^[a-z0-9-]{3,50}$";

    public CreateTenantCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Slug)
            .NotEmpty()
            .Matches(SlugPattern)
            .WithMessage("Slug must be 3-50 chars and contain only lowercase letters, numbers, and hyphens.");
    }
}
