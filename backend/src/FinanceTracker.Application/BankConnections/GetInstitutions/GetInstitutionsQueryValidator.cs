using FluentValidation;

namespace FinanceTracker.Application.BankConnections.GetInstitutions;

internal sealed class GetInstitutionsQueryValidator : AbstractValidator<GetInstitutionsQuery>
{
    public GetInstitutionsQueryValidator()
    {
        RuleFor(x => x.CountryCode)
            .NotEmpty()
            .Length(2);
    }
}
