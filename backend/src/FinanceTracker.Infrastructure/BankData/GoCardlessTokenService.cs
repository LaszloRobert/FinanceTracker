using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace FinanceTracker.Infrastructure.BankData;

internal sealed class GoCardlessTokenService(
    HttpClient httpClient,
    IOptions<GoCardlessOptions> options,
    GoCardlessTokenCache cache)
{
    private readonly GoCardlessOptions _options = options.Value;

    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        if (cache.IsAccessTokenValid)
        {
            return cache.AccessToken!;
        }

        return await cache.WithLockAsync(async () =>
        {
            // Double-check after acquiring lock
            if (cache.IsAccessTokenValid)
            {
                return cache.AccessToken!;
            }

            if (cache.IsRefreshTokenValid)
            {
                return await RefreshAccessTokenAsync(cancellationToken);
            }

            return await CreateNewTokenAsync(cancellationToken);
        }, cancellationToken);
    }

    private async Task<string> CreateNewTokenAsync(CancellationToken cancellationToken)
    {
        var request = new { secret_id = _options.SecretId, secret_key = _options.SecretKey };

        HttpResponseMessage response = await httpClient.PostAsJsonAsync(
            "/api/v2/token/new/",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        TokenResponse? tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken);

        cache.AccessToken = tokenResponse!.Access;
        cache.AccessTokenExpiry = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.AccessExpires - 60);
        cache.RefreshToken = tokenResponse.Refresh;
        cache.RefreshTokenExpiry = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.RefreshExpires - 60);

        return cache.AccessToken;
    }

    private async Task<string> RefreshAccessTokenAsync(CancellationToken cancellationToken)
    {
        var request = new { refresh = cache.RefreshToken };

        HttpResponseMessage response = await httpClient.PostAsJsonAsync(
            "/api/v2/token/refresh/",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        TokenRefreshResponse? tokenResponse = await response.Content.ReadFromJsonAsync<TokenRefreshResponse>(cancellationToken);

        cache.AccessToken = tokenResponse!.Access;
        cache.AccessTokenExpiry = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.AccessExpires - 60);

        return cache.AccessToken;
    }

    private sealed record TokenResponse(
        [property: JsonPropertyName("access")] string Access,
        [property: JsonPropertyName("access_expires")] int AccessExpires,
        [property: JsonPropertyName("refresh")] string Refresh,
        [property: JsonPropertyName("refresh_expires")] int RefreshExpires);

    private sealed record TokenRefreshResponse(
        [property: JsonPropertyName("access")] string Access,
        [property: JsonPropertyName("access_expires")] int AccessExpires);
}
