using FluentValidation;

namespace FinanceTracker.Application.Categories.Create;

internal sealed class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(x => x.Icon)
            .MaximumLength(64)
            .When(x => x.Icon is not null);

        RuleFor(x => x.Color)
            .MaximumLength(16)
            .When(x => x.Color is not null);
    }
}
