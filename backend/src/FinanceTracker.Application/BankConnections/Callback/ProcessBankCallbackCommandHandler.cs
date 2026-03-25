using FinanceTracker.Application.Abstractions.Authentication;
using FinanceTracker.Application.Abstractions.BankData;
using FinanceTracker.Application.Abstractions.BankData.Models;
using FinanceTracker.Application.Abstractions.Data;
using FinanceTracker.Application.Abstractions.Messaging;
using FinanceTracker.Domain.Accounts;
using FinanceTracker.Domain.BankConnections;
using FinanceTracker.SharedKernel;

namespace FinanceTracker.Application.BankConnections.Callback;

internal sealed class ProcessBankCallbackCommandHandler(
    IBankDataService bankDataService,
    IApplicationDbContext dbContext,
    IUserContext userContext,
    TimeProvider timeProvider)
    : ICommandHandler<ProcessBankCallbackCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        ProcessBankCallbackCommand command,
        CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;
        DateTimeOffset now = timeProvider.GetUtcNow();

        RequisitionDetails requisition = await bankDataService.GetRequisitionDetailsAsync(
            command.RequisitionId,
            cancellationToken);

        if (requisition.Status != RequisitionStatus.Linked)
        {
            return Result.Failure<Guid>(
                Error.Problem("BankConnections.NotLinked", $"Requisition status is '{requisition.Status}', expected 'Linked'."));
        }

        // Get institution details for name and logo
        BankInstitution? institution = await bankDataService.GetInstitutionByIdAsync(
            requisition.InstitutionId,
            cancellationToken);

        var bankConnection = BankConnection.Create(
            userId,
            requisition.InstitutionId,
            institution?.Name ?? requisition.InstitutionId,
            institution?.Logo,
            command.RequisitionId,
            institution?.MaxAccessValidForDays ?? 90,
            now);

        dbContext.BankConnections.Add(bankConnection);

        foreach (string accountId in requisition.AccountIds)
        {
            BankAccountDetails details = await bankDataService.GetAccountDetailsAsync(
                accountId,
                cancellationToken);

            var account = Account.Create(
                userId,
                bankConnection.Id,
                accountId,
                details.Currency,
                details.Iban,
                details.OwnerName,
                details.Product,
                now);

            dbContext.Accounts.Add(account);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return bankConnection.Id;
    }
}
