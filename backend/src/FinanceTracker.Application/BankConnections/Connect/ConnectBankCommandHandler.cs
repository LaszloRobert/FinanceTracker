using FinanceTracker.Application.Abstractions.BankData;
using FinanceTracker.Application.Abstractions.BankData.Models;
using FinanceTracker.Application.Abstractions.Messaging;
using FinanceTracker.SharedKernel;

namespace FinanceTracker.Application.BankConnections.Connect;

internal sealed class ConnectBankCommandHandler(IBankDataService bankDataService)
    : ICommandHandler<ConnectBankCommand, ConnectBankResponse>
{
    public async Task<Result<ConnectBankResponse>> Handle(
        ConnectBankCommand command,
        CancellationToken cancellationToken)
    {
        RequisitionResult requisition = await bankDataService.CreateRequisitionAsync(
            command.InstitutionId,
            command.RedirectUrl,
            cancellationToken);

        var response = new ConnectBankResponse(requisition.RequisitionId, requisition.Link);

        return response;
    }
}
