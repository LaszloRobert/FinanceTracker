using FinanceTracker.Application.Abstractions.Authentication;
using FinanceTracker.Application.Abstractions.Data;
using FinanceTracker.Application.Abstractions.Messaging;
using FinanceTracker.Domain.Accounts;
using FinanceTracker.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Application.Accounts.UpdateDisplayName;

internal sealed class UpdateDisplayNameCommandHandler(
    IApplicationDbContext dbContext,
    IUserContext userContext,
    TimeProvider timeProvider)
    : ICommandHandler<UpdateDisplayNameCommand>
{
    public async Task<Result> Handle(
        UpdateDisplayNameCommand command,
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
            return Result.Failure(AccountErrors.NotFound(command.AccountId));
        }

        account.UpdateDisplayName(command.DisplayName, now);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
