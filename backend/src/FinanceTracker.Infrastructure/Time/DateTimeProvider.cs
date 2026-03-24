using FinanceTracker.Application.Abstractions.Time;

namespace FinanceTracker.Infrastructure.Time;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
