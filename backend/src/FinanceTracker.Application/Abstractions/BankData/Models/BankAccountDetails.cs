namespace FinanceTracker.Application.Abstractions.BankData.Models;

public sealed record BankAccountDetails(
    string? Iban,
    string Currency,
    string? OwnerName,
    string? DisplayName,
    string? Product);
