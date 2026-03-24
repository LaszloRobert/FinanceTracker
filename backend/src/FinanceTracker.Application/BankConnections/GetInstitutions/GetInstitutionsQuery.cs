using FinanceTracker.Application.Abstractions.Messaging;

namespace FinanceTracker.Application.BankConnections.GetInstitutions;

public sealed record GetInstitutionsQuery(string CountryCode) : IQuery<List<InstitutionResponse>>;

public sealed record InstitutionResponse(
    string Id,
    string Name,
    string? Logo,
    int TransactionTotalDays,
    int MaxAccessValidForDays);
