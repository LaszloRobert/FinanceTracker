using FinanceTracker.Application.Abstractions.Messaging;

namespace FinanceTracker.Application.BankConnections.GetAll;

public sealed record GetBankConnectionsQuery : IQuery<List<BankConnectionResponse>>;

public sealed record BankConnectionResponse(
    Guid Id,
    string InstitutionId,
    string InstitutionName,
    string? InstitutionLogo,
    string Status,
    DateTimeOffset LinkedAt,
    DateTimeOffset ExpiresAt,
    int AccountCount);
