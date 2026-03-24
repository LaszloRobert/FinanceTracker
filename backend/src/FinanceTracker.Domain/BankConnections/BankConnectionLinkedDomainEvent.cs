using FinanceTracker.SharedKernel;

namespace FinanceTracker.Domain.BankConnections;

public sealed record BankConnectionLinkedDomainEvent(Guid BankConnectionId) : IDomainEvent;
