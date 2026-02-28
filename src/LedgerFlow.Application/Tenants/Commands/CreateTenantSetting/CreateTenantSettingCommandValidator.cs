using FluentValidation;

namespace LedgerFlow.Application.Tenants.Commands.CreateTenantSetting;

public sealed class CreateTenantSettingCommandValidator : AbstractValidator<CreateTenantSettingCommand>
{
    public CreateTenantSettingCommandValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Value)
            .NotEmpty()
            .MaximumLength(2000);
    }
}
