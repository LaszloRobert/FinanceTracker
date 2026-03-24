using FinanceTracker.Application.Abstractions.Messaging;

namespace FinanceTracker.Application.Accounts.GetAll;

public sealed record GetAccountsQuery : IQuery<List<AccountResponse>>;

public sealed record AccountResponse(
    Guid Id,
    Guid BankConnectionId,
    string? Iban,
    string Currency,
    string? OwnerName,
    string? DisplayName,
    string? Product,
    DateTimeOffset? LastSyncedAt);
