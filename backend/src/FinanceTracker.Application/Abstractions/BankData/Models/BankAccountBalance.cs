namespace FinanceTracker.Application.Abstractions.BankData.Models;

public sealed record BankAccountBalance(
    decimal Amount,
    string Currency,
    string BalanceType,
    DateOnly? ReferenceDate);
