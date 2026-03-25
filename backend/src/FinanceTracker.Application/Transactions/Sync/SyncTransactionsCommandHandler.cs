using FinanceTracker.Application.Abstractions.Authentication;
using FinanceTracker.Application.Abstractions.BankData;
using FinanceTracker.Application.Abstractions.BankData.Models;
using FinanceTracker.Application.Abstractions.Data;
using FinanceTracker.Application.Abstractions.Messaging;
using FinanceTracker.Domain.Accounts;
using FinanceTracker.Domain.Transactions;
using FinanceTracker.Domain.ValueObjects;
using FinanceTracker.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Application.Transactions.Sync;

internal sealed class SyncTransactionsCommandHandler(
    IBankDataService bankDataService,
    IApplicationDbContext dbContext,
    IUserContext userContext,
    TimeProvider timeProvider)
    : ICommandHandler<SyncTransactionsCommand, int>
{
    public async Task<Result<int>> Handle(
        SyncTransactionsCommand command,
        CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;
        DateTimeOffset now = timeProvider.GetUtcNow();

        Account? account = await dbContext.Accounts
            .FirstOrDefaultAsync(
                a => a.Id == command.AccountId && a.UserId == userId,
                cancellationToken);

        if (account is null)
        {
            return Result.Failure<int>(AccountErrors.NotFound(command.AccountId));
        }

        // Fetch transactions from GoCardless
        DateOnly? dateFrom = account.LastSyncedAt.HasValue
            ? DateOnly.FromDateTime(account.LastSyncedAt.Value.UtcDateTime)
            : null;

        BankTransactionResult bankTransactions = await bankDataService.GetTransactionsAsync(
            account.ExternalId,
            dateFrom,
            null,
            cancellationToken);

        // Get existing transaction IDs for deduplication
        var existingExternalIds = (await dbContext.Transactions
            .Where(t => t.AccountId == account.Id && t.ExternalId != null)
            .Select(t => t.ExternalId!)
            .ToListAsync(cancellationToken))
            .ToHashSet();

        var existingInternalIds = (await dbContext.Transactions
            .Where(t => t.AccountId == account.Id && t.InternalId != null)
            .Select(t => t.InternalId!)
            .ToListAsync(cancellationToken))
            .ToHashSet();

        // Pre-load all pending transactions for matching (avoids N+1)
        List<Transaction> pendingTransactions = await dbContext.Transactions
            .Where(t => t.AccountId == account.Id
                && t.Status == TransactionStatus.Pending
                && t.ReplacedByTransactionId == null)
            .ToListAsync(cancellationToken);

        int newCount = 0;

        // Process booked transactions
        foreach (BankTransaction bt in bankTransactions.Booked)
        {
            if (IsAlreadySynced(bt, existingExternalIds, existingInternalIds))
            {
                continue;
            }

            Transaction transaction = MapToTransaction(bt, account.Id, userId, TransactionStatus.Booked, now);
            dbContext.Transactions.Add(transaction);
            newCount++;

            // Try to match and replace a pending transaction
            Transaction? pendingMatch = pendingTransactions.FirstOrDefault(
                t => t.TransactionAmount.Amount == bt.Amount
                    && t.TransactionAmount.Currency == bt.Currency
                    && t.RemittanceInfo == bt.RemittanceInfo);

            if (pendingMatch is not null)
            {
                pendingMatch.MarkReplacedBy(transaction.Id, now);
                pendingTransactions.Remove(pendingMatch);
            }
        }

        // Process pending transactions
        foreach (BankTransaction bt in bankTransactions.Pending)
        {
            if (IsAlreadySynced(bt, existingExternalIds, existingInternalIds))
            {
                continue;
            }

            // Check if a similar pending already exists in memory
            bool pendingExists = pendingTransactions.Any(
                t => t.TransactionAmount.Amount == bt.Amount
                    && t.RemittanceInfo == bt.RemittanceInfo);

            if (pendingExists)
            {
                continue;
            }

            Transaction transaction = MapToTransaction(bt, account.Id, userId, TransactionStatus.Pending, now);
            dbContext.Transactions.Add(transaction);
            newCount++;
        }

        // Update balances
        List<BankAccountBalance> balances = await bankDataService.GetAccountBalancesAsync(
            account.ExternalId,
            cancellationToken);

        var accountBalances = balances
            .Select(b => new AccountBalance
            {
                Amount = b.Amount,
                Currency = b.Currency,
                BalanceType = b.BalanceType,
                ReferenceDate = b.ReferenceDate,
            })
            .ToList();

        account.UpdateBalances(accountBalances, now);
        account.MarkSynced(now);

        await dbContext.SaveChangesAsync(cancellationToken);

        return newCount;
    }

    private static bool IsAlreadySynced(
        BankTransaction bt,
        HashSet<string> existingExternalIds,
        HashSet<string> existingInternalIds)
    {
        if (bt.TransactionId is not null && existingExternalIds.Contains(bt.TransactionId))
        {
            return true;
        }

        if (bt.InternalTransactionId is not null && existingInternalIds.Contains(bt.InternalTransactionId))
        {
            return true;
        }

        return false;
    }

    private static Transaction MapToTransaction(
        BankTransaction bt,
        Guid accountId,
        Guid userId,
        TransactionStatus status,
        DateTimeOffset now)
    {
        var money = new Money(bt.Amount, bt.Currency);

        return Transaction.Create(
            accountId,
            userId,
            money,
            status,
            bt.TransactionId,
            bt.InternalTransactionId,
            bt.BookingDate,
            bt.ValueDate,
            bt.Creditor?.Name,
            bt.Creditor?.Iban,
            bt.Debtor?.Name,
            bt.Debtor?.Iban,
            bt.RemittanceInfo,
            bt.AdditionalInfo,
            bt.MerchantCategoryCode,
            bt.BankTransactionCode,
            now);
    }
}
