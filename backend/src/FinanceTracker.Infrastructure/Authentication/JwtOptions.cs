using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Infrastructure.Authentication;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required]
    [MinLength(32)]
    public required string Secret { get; init; }

    [Required]
    public required string Issuer { get; init; }

    [Required]
    public required string Audience { get; init; }

    [Range(1, 10080)]
    public int ExpirationInMinutes { get; init; } = 60;
}
