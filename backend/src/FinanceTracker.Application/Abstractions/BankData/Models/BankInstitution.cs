namespace FinanceTracker.Application.Abstractions.BankData.Models;

public sealed record BankInstitution(
    string Id,
    string Name,
    string? Logo,
    int TransactionTotalDays,
    int MaxAccessValidForDays,
    List<string> Countries);
