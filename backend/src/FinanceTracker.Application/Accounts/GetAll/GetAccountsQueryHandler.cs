using FinanceTracker.Application.Abstractions.Authentication;
using FinanceTracker.Application.Abstractions.Data;
using FinanceTracker.Application.Abstractions.Messaging;
using FinanceTracker.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Application.Accounts.GetAll;

internal sealed class GetAccountsQueryHandler(
    IApplicationDbContext dbContext,
    IUserContext userContext)
    : IQueryHandler<GetAccountsQuery, List<AccountResponse>>
{
    public async Task<Result<List<AccountResponse>>> Handle(
        GetAccountsQuery query,
        CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;

        List<AccountResponse> accounts = await dbContext.Accounts
            .Where(a => a.UserId == userId)
            .Select(a => new AccountResponse(
                a.Id,
                a.BankConnectionId,
                a.Iban,
                a.Currency,
                a.OwnerName,
                a.DisplayName,
                a.Product,
                a.LastSyncedAt))
            .ToListAsync(cancellationToken);

        return accounts;
    }
}
