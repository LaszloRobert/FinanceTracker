using FinanceTracker.Application.Abstractions.Messaging;

namespace FinanceTracker.Application.BankConnections.Connect;

public sealed record ConnectBankCommand(string InstitutionId, Uri RedirectUrl) : ICommand<ConnectBankResponse>;

public sealed record ConnectBankResponse(string RequisitionId, string AuthLink);
