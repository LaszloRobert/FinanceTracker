using FinanceTracker.Application.Abstractions.Authentication;
using FinanceTracker.Application.Abstractions.Data;
using FinanceTracker.Application.Abstractions.Messaging;
using FinanceTracker.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Application.Transactions.GetAll;

internal sealed class GetTransactionsQueryHandler(
    IApplicationDbContext dbContext,
    IUserContext userContext) : IQueryHandler<GetTransactionsQuery, TransactionsPageResponse>
{
    public async Task<Result<TransactionsPageResponse>> Handle(
        GetTransactionsQuery query,
        CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;

        IQueryable<Domain.Transactions.Transaction> baseQuery = dbContext.Transactions
            .Where(t => t.UserId == userId)
            .Where(t => t.ReplacedByTransactionId == null);

        if (query.AccountId.HasValue)
        {
            baseQuery = baseQuery.Where(t => t.AccountId == query.AccountId.Value);
        }

        if (query.CategoryId.HasValue)
        {
            baseQuery = baseQuery.Where(t => t.CategoryId == query.CategoryId.Value);
        }

        if (query.DateFrom.HasValue)
        {
            baseQuery = baseQuery.Where(t => t.BookingDate >= query.DateFrom.Value);
        }

        if (query.DateTo.HasValue)
        {
            baseQuery = baseQuery.Where(t => t.BookingDate <= query.DateTo.Value);
        }

        int totalCount = await baseQuery.CountAsync(cancellationToken);

        List<TransactionResponse> items = await baseQuery
            .OrderByDescending(t => t.BookingDate)
            .ThenByDescending(t => t.ValueDate)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .GroupJoin(
                dbContext.Categories,
                t => t.CategoryId,
                c => c.Id,
                (t, categories) => new { Transaction = t, Categories = categories })
            .SelectMany(
                x => x.Categories.DefaultIfEmpty(),
                (x, category) => new TransactionResponse(
                    x.Transaction.Id,
                    x.Transaction.AccountId,
                    x.Transaction.TransactionAmount.Amount,
                    x.Transaction.TransactionAmount.Currency,
                    x.Transaction.BookingDate,
                    x.Transaction.ValueDate,
                    x.Transaction.CreditorName,
                    x.Transaction.DebtorName,
                    x.Transaction.RemittanceInfo,
                    x.Transaction.Status.ToString(),
                    x.Transaction.CategoryId,
                    category != null ? category.Name : null))
            .ToListAsync(cancellationToken);

        var response = new TransactionsPageResponse(items, totalCount, query.Page, query.PageSize);

        return Result.Success(response);
    }
}
