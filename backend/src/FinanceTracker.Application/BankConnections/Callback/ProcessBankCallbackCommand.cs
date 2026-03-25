using FinanceTracker.Application.Abstractions.Messaging;

namespace FinanceTracker.Application.BankConnections.Callback;

public sealed record ProcessBankCallbackCommand(string RequisitionId) : ICommand<Guid>;
