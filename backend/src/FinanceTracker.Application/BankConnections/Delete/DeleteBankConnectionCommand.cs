using FinanceTracker.Application.Abstractions.Messaging;

namespace FinanceTracker.Application.BankConnections.Delete;

public sealed record DeleteBankConnectionCommand(Guid BankConnectionId) : ICommand;
