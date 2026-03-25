namespace FinanceTracker.Infrastructure.BankData;

internal sealed class GoCardlessTokenCache : IDisposable
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public string? AccessToken { get; set; }
    public DateTimeOffset AccessTokenExpiry { get; set; }
    public string? RefreshToken { get; set; }
    public DateTimeOffset RefreshTokenExpiry { get; set; }

    public bool IsAccessTokenValid => AccessToken is not null && DateTimeOffset.UtcNow < AccessTokenExpiry;

    public bool IsRefreshTokenValid => RefreshToken is not null && DateTimeOffset.UtcNow < RefreshTokenExpiry;

    public async Task<T> WithLockAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            return await action();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void Dispose() => _semaphore.Dispose();
}
