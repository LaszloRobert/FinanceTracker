using FinanceTracker.Application.Abstractions.Messaging;

namespace FinanceTracker.Application.Accounts.UpdateDisplayName;

public sealed record UpdateDisplayNameCommand(Guid AccountId, string DisplayName) : ICommand;
