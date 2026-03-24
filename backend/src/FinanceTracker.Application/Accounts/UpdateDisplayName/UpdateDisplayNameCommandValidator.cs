using FluentValidation;

namespace FinanceTracker.Application.Accounts.UpdateDisplayName;

internal sealed class UpdateDisplayNameCommandValidator : AbstractValidator<UpdateDisplayNameCommand>
{
    public UpdateDisplayNameCommandValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty();

        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .MaximumLength(128);
    }
}
