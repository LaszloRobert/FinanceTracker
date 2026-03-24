namespace FinanceTracker.Domain.ValueObjects;

public sealed record Money(decimal Amount, string Currency)
{
    public static Money Zero(string currency) => new(0, currency);

    public bool IsNegative => Amount < 0;

    public bool IsPositive => Amount > 0;
}
