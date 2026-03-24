using FinanceTracker.Application.Abstractions.BankData;
using FinanceTracker.Application.Abstractions.Messaging;
using FinanceTracker.SharedKernel;

namespace FinanceTracker.Application.BankConnections.Connect;

internal sealed class ConnectBankCommandHandler(IBankDataService bankDataService)
    : ICommandHandler<ConnectBankCommand, string>
{
    public async Task<Result<string>> Handle(
        ConnectBankCommand command,
        CancellationToken cancellationToken)
    {
        string authLink = await bankDataService.CreateRequisitionAsync(
            command.InstitutionId,
            command.RedirectUrl,
            cancellationToken);

        return authLink;
    }
}
