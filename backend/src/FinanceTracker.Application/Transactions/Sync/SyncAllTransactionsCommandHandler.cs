using FinanceTracker.Application.Abstractions.Authentication;
using FinanceTracker.Application.Abstractions.Data;
using FinanceTracker.Application.Abstractions.Messaging;
using FinanceTracker.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FinanceTracker.Application.Transactions.Sync;

internal sealed class SyncAllTransactionsCommandHandler(
    IApplicationDbContext dbContext,
    IUserContext userContext,
    ICommandHandler<SyncTransactionsCommand, int> syncHandler,
    ILogger<SyncAllTransactionsCommandHandler> logger)
    : ICommandHandler<SyncAllTransactionsCommand, int>
{
    public async Task<Result<int>> Handle(
        SyncAllTransactionsCommand command,
        CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;

        List<Guid> accountIds = await dbContext.Accounts
            .Where(a => a.UserId == userId)
            .Select(a => a.Id)
            .ToListAsync(cancellationToken);

        int totalSynced = 0;
        int failedCount = 0;

        foreach (Guid accountId in accountIds)
        {
            Result<int> result = await syncHandler.Handle(
                new SyncTransactionsCommand(accountId),
                cancellationToken);

            if (result.IsSuccess)
            {
                totalSynced += result.Value;
            }
            else
            {
                failedCount++;
                logger.LogWarning(
                    "Failed to sync account {AccountId}: {Error}",
                    accountId,
                    result.Error.Description);
            }
        }

        if (failedCount > 0)
        {
            logger.LogWarning(
                "Sync completed with {FailedCount} failures out of {TotalCount} accounts",
                failedCount,
                accountIds.Count);
        }

        return totalSynced;
    }
}
