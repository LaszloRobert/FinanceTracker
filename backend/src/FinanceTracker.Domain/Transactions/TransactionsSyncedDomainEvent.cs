using FinanceTracker.SharedKernel;

namespace FinanceTracker.Domain.Transactions;

public sealed record TransactionsSyncedDomainEvent(Guid AccountId, int Count) : IDomainEvent;
