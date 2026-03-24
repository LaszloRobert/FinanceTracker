using FinanceTracker.Application.Abstractions.Authentication;
using FinanceTracker.Application.Abstractions.Data;
using FinanceTracker.Application.Abstractions.Messaging;
using FinanceTracker.Domain.BankConnections;
using FinanceTracker.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Application.BankConnections.Delete;

internal sealed class DeleteBankConnectionCommandHandler(
    IApplicationDbContext dbContext,
    IUserContext userContext,
    TimeProvider timeProvider)
    : ICommandHandler<DeleteBankConnectionCommand>
{
    public async Task<Result> Handle(
        DeleteBankConnectionCommand command,
        CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;
        DateTimeOffset now = timeProvider.GetUtcNow();

        BankConnection? bankConnection = await dbContext.BankConnections
            .FirstOrDefaultAsync(
                bc => bc.Id == command.BankConnectionId && bc.UserId == userId && bc.DeletedAt == null,
                cancellationToken);

        if (bankConnection is null)
        {
            return Result.Failure(BankConnectionErrors.NotFound(command.BankConnectionId));
        }

        bankConnection.SoftDelete(now);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
