using FinanceTracker.Application.Abstractions.Authentication;
using FinanceTracker.Application.Abstractions.Data;
using FinanceTracker.Application.Abstractions.Messaging;
using FinanceTracker.Domain.Transactions;
using FinanceTracker.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Application.Transactions.Categorize;

internal sealed class CategorizeTransactionCommandHandler(
    IApplicationDbContext dbContext,
    IUserContext userContext,
    TimeProvider timeProvider) : ICommandHandler<CategorizeTransactionCommand>
{
    public async Task<Result> Handle(
        CategorizeTransactionCommand command,
        CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;
        DateTimeOffset now = timeProvider.GetUtcNow();

        Transaction? transaction = await dbContext.Transactions
            .FirstOrDefaultAsync(
                t => t.Id == command.TransactionId && t.UserId == userId,
                cancellationToken);

        if (transaction is null)
        {
            return Result.Failure(TransactionErrors.NotFound(command.TransactionId));
        }

        if (command.CategoryId is null)
        {
            transaction.RemoveCategory(now);
        }
        else
        {
            bool categoryExists = await dbContext.Categories
                .AnyAsync(
                    c => c.Id == command.CategoryId.Value && c.UserId == userId,
                    cancellationToken);

            if (!categoryExists)
            {
                return Result.Failure(
                    Error.NotFound("Categories.NotFound", $"Category with id '{command.CategoryId.Value}' was not found."));
            }

            transaction.Categorize(command.CategoryId.Value, now);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
