namespace FinanceTracker.Domain.ValueObjects;

public sealed record AccountBalance(
    Money Amount,
    string BalanceType,
    DateOnly? ReferenceDate);
