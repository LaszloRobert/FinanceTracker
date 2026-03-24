namespace FinanceTracker.Application.Abstractions.BankData.Models;

public sealed record BankTransaction(
    string? TransactionId,
    string? InternalTransactionId,
    decimal Amount,
    string Currency,
    DateOnly? BookingDate,
    DateOnly? ValueDate,
    BankTransactionParty? Creditor,
    BankTransactionParty? Debtor,
    string? RemittanceInfo,
    string? AdditionalInfo,
    string? MerchantCategoryCode,
    string? BankTransactionCode,
    bool IsBooked);
