using FinanceTracker.SharedKernel;

namespace FinanceTracker.Domain.BankConnections;

public sealed record BankConnectionErrorDomainEvent(Guid BankConnectionId) : IDomainEvent;
