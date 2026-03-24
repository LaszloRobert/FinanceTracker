using FluentValidation;

namespace FinanceTracker.Application.BankConnections.Connect;

internal sealed class ConnectBankCommandValidator : AbstractValidator<ConnectBankCommand>
{
    public ConnectBankCommandValidator()
    {
        RuleFor(x => x.InstitutionId)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(x => x.RedirectUrl)
            .NotNull();
    }
}
