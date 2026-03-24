using FinanceTracker.Application.Abstractions.Messaging;

namespace FinanceTracker.Application.Transactions.Categorize;

public sealed record CategorizeTransactionCommand(Guid TransactionId, Guid? CategoryId) : ICommand;
