using FinanceTracker.Application.Abstractions.BankData.Models;

namespace FinanceTracker.Application.Abstractions.BankData;

public interface IBankDataService
{
    Task<RequisitionResult> CreateRequisitionAsync(string institutionId, Uri redirectUrl, CancellationToken cancellationToken);

    Task<RequisitionDetails> GetRequisitionDetailsAsync(string requisitionId, CancellationToken cancellationToken);

    Task<BankAccountDetails> GetAccountDetailsAsync(string accountId, CancellationToken cancellationToken);

    Task<List<BankAccountBalance>> GetAccountBalancesAsync(string accountId, CancellationToken cancellationToken);

    Task<BankTransactionResult> GetTransactionsAsync(string accountId, DateOnly? dateFrom, DateOnly? dateTo, CancellationToken cancellationToken);

    Task<List<BankInstitution>> GetInstitutionsAsync(string countryCode, CancellationToken cancellationToken);

    Task<BankInstitution?> GetInstitutionByIdAsync(string institutionId, CancellationToken cancellationToken);
}
