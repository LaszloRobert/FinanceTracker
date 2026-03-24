using FinanceTracker.Application.Abstractions.BankData;
using FinanceTracker.Application.Abstractions.BankData.Models;
using FinanceTracker.Application.Abstractions.Messaging;
using FinanceTracker.SharedKernel;

namespace FinanceTracker.Application.BankConnections.GetInstitutions;

internal sealed class GetInstitutionsQueryHandler(IBankDataService bankDataService)
    : IQueryHandler<GetInstitutionsQuery, List<InstitutionResponse>>
{
    public async Task<Result<List<InstitutionResponse>>> Handle(
        GetInstitutionsQuery query,
        CancellationToken cancellationToken)
    {
        List<BankInstitution> institutions = await bankDataService.GetInstitutionsAsync(
            query.CountryCode,
            cancellationToken);

        var response = institutions
            .Select(i => new InstitutionResponse(
                i.Id,
                i.Name,
                i.Logo,
                i.TransactionTotalDays,
                i.MaxAccessValidForDays))
            .ToList();

        return response;
    }
}
