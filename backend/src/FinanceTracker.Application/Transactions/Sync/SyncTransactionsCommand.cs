using FinanceTracker.Application.Abstractions.Messaging;

namespace FinanceTracker.Application.Transactions.Sync;

public sealed record SyncTransactionsCommand(Guid AccountId) : ICommand<int>;
