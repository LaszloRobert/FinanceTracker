namespace FinanceTracker.Application.Abstractions.BankData.Models;

public sealed record BankTransactionResult(
    List<BankTransaction> Booked,
    List<BankTransaction> Pending);
