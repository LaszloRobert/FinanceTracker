using FinanceTracker.SharedKernel;

namespace FinanceTracker.Domain.Users;

public sealed record UserRegisteredDomainEvent(Guid UserId) : IDomainEvent;
