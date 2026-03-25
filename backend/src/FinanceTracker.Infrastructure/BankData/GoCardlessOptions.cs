using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Infrastructure.BankData;

internal sealed class GoCardlessOptions
{
    public const string SectionName = "GoCardless";

    [Required]
    public required string SecretId { get; init; }

    [Required]
    public required string SecretKey { get; init; }

    public string BaseUrl { get; init; } = "https://bankaccountdata.gocardless.com";
}
