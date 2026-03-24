using FinanceTracker.Application.Abstractions.Messaging;

namespace FinanceTracker.Application.Transactions.GetAll;

public sealed record GetTransactionsQuery(
    Guid? AccountId,
    Guid? CategoryId,
    DateOnly? DateFrom,
    DateOnly? DateTo,
    int Page,
    int PageSize) : IQuery<TransactionsPageResponse>;

public sealed record TransactionsPageResponse(
    List<TransactionResponse> Items,
    int TotalCount,
    int Page,
    int PageSize);

public sealed record TransactionResponse(
    Guid Id,
    Guid AccountId,
    decimal Amount,
    string Currency,
    DateOnly? BookingDate,
    DateOnly? ValueDate,
    string? CreditorName,
    string? DebtorName,
    string? RemittanceInfo,
    string Status,
    Guid? CategoryId,
    string? CategoryName);
