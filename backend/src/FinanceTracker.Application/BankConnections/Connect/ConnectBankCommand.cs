using FinanceTracker.Application.Abstractions.Messaging;

namespace FinanceTracker.Application.BankConnections.Connect;

public sealed record ConnectBankCommand(string InstitutionId, Uri RedirectUrl) : ICommand<string>;
