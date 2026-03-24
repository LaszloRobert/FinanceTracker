using FinanceTracker.SharedKernel;

namespace FinanceTracker.Domain.BankConnections;

public sealed record BankConnectionExpiredDomainEvent(Guid BankConnectionId) : IDomainEvent;
