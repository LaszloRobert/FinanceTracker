using FinanceTracker.Application.Abstractions.BankData.Models;

namespace FinanceTracker.Application.Abstractions.BankData;

public interface IBankDataService
{
    Task<string> CreateRequisitionAsync(string institutionId, Uri redirectUrl, CancellationToken cancellationToken);

    Task<RequisitionStatus> GetRequisitionStatusAsync(string requisitionId, CancellationToken cancellationToken);

    Task<List<string>> GetAccountIdsAsync(string requisitionId, CancellationToken cancellationToken);

    Task<BankAccountDetails> GetAccountDetailsAsync(string accountId, CancellationToken cancellationToken);

    Task<List<BankAccountBalance>> GetAccountBalancesAsync(string accountId, CancellationToken cancellationToken);

    Task<BankTransactionResult> GetTransactionsAsync(string accountId, DateOnly? dateFrom, DateOnly? dateTo, CancellationToken cancellationToken);

    Task<List<BankInstitution>> GetInstitutionsAsync(string countryCode, CancellationToken cancellationToken);
}
