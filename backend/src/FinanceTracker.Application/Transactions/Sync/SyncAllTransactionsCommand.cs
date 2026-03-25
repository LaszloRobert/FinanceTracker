using FinanceTracker.Application.Abstractions.Messaging;

namespace FinanceTracker.Application.Transactions.Sync;

public sealed record SyncAllTransactionsCommand : ICommand<int>;
