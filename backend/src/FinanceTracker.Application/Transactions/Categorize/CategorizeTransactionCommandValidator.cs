using FluentValidation;

namespace FinanceTracker.Application.Transactions.Categorize;

internal sealed class CategorizeTransactionCommandValidator : AbstractValidator<CategorizeTransactionCommand>
{
    public CategorizeTransactionCommandValidator()
    {
        RuleFor(x => x.TransactionId)
            .NotEmpty();
    }
}
