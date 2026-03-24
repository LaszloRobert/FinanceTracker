using FinanceTracker.Domain.ValueObjects;
using FinanceTracker.SharedKernel;

namespace FinanceTracker.Domain.Transactions;

public sealed class Transaction : Entity
{
    public Guid AccountId { get; private set; }
    public Guid UserId { get; private set; }

    // Deduplication identifiers
    public string? ExternalId { get; private set; }
    public string? InternalId { get; private set; }

    // Amount
    public Money TransactionAmount { get; private set; } = default!;

    // Dates
    public DateOnly? BookingDate { get; private set; }
    public DateOnly? ValueDate { get; private set; }

    // Parties
    public string? CreditorName { get; private set; }
    public string? CreditorIban { get; private set; }
    public string? DebtorName { get; private set; }
    public string? DebtorIban { get; private set; }

    // Description
    public string? RemittanceInfo { get; private set; }
    public string? AdditionalInfo { get; private set; }

    // Metadata
    public TransactionStatus Status { get; private set; }
    public string? MerchantCategoryCode { get; private set; }
    public string? BankTransactionCode { get; private set; }

    // Pending to Booked reconciliation
    public Guid? ReplacedByTransactionId { get; private set; }
    public bool IsReplaced => ReplacedByTransactionId.HasValue;

    // Categorization
    public Guid? CategoryId { get; private set; }

    private Transaction() { }

    public static Transaction Create(
        Guid accountId,
        Guid userId,
        Money transactionAmount,
        TransactionStatus status,
        string? externalId,
        string? internalId,
        DateOnly? bookingDate,
        DateOnly? valueDate,
        string? creditorName,
        string? creditorIban,
        string? debtorName,
        string? debtorIban,
        string? remittanceInfo,
        string? additionalInfo,
        string? merchantCategoryCode,
        string? bankTransactionCode,
        DateTime now)
    {
        return new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            UserId = userId,
            TransactionAmount = transactionAmount,
            Status = status,
            ExternalId = externalId,
            InternalId = internalId,
            BookingDate = bookingDate,
            ValueDate = valueDate,
            CreditorName = creditorName,
            CreditorIban = creditorIban,
            DebtorName = debtorName,
            DebtorIban = debtorIban,
            RemittanceInfo = remittanceInfo,
            AdditionalInfo = additionalInfo,
            MerchantCategoryCode = merchantCategoryCode,
            BankTransactionCode = bankTransactionCode,
            CreatedAt = now
        };
    }

    public void Categorize(Guid categoryId, DateTime now)
    {
        CategoryId = categoryId;
        UpdatedAt = now;
    }

    public void RemoveCategory(DateTime now)
    {
        CategoryId = null;
        UpdatedAt = now;
    }

    public void MarkReplacedBy(Guid bookedTransactionId, DateTime now)
    {
        ReplacedByTransactionId = bookedTransactionId;
        UpdatedAt = now;
    }

    public void PromoteToBooked(string externalId, DateOnly bookingDate, DateTime now)
    {
        Status = TransactionStatus.Booked;
        ExternalId = externalId;
        BookingDate = bookingDate;
        UpdatedAt = now;
    }
}
