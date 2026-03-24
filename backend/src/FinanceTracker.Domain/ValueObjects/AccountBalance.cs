namespace FinanceTracker.Domain.ValueObjects;

public sealed class AccountBalance
{
    public decimal Amount { get; init; }

    public string Currency { get; init; } = default!;

    public string BalanceType { get; init; } = default!;

    public DateOnly? ReferenceDate { get; init; }
}
