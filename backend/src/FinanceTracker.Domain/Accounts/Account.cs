using FinanceTracker.Domain.ValueObjects;
using FinanceTracker.SharedKernel;

namespace FinanceTracker.Domain.Accounts;

public sealed class Account : Entity
{
    public Guid UserId { get; private set; }
    public Guid BankConnectionId { get; private set; }
    public string ExternalId { get; private set; } = default!;
    public string? Iban { get; private set; }
    public string Currency { get; private set; } = default!;
    public string? OwnerName { get; private set; }
    public string? DisplayName { get; private set; }
    public string? Product { get; private set; }
    public DateTimeOffset? LastSyncedAt { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

    public List<AccountBalance> Balances { get; private set; } = [];

    private Account() { }

    public static Account Create(
        Guid userId,
        Guid bankConnectionId,
        string externalId,
        string currency,
        string? iban,
        string? ownerName,
        string? product,
        DateTimeOffset now)
    {
        return new Account
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            BankConnectionId = bankConnectionId,
            ExternalId = externalId,
            Currency = currency,
            Iban = iban,
            OwnerName = ownerName,
            Product = product,
            CreatedAt = now
        };
    }

    public void UpdateDisplayName(string displayName, DateTimeOffset now)
    {
        DisplayName = displayName;
        UpdatedAt = now;
    }

    public void UpdateBalances(List<AccountBalance> balances, DateTimeOffset now)
    {
        Balances.Clear();
        Balances.AddRange(balances);
        UpdatedAt = now;
    }

    public void MarkSynced(DateTimeOffset now)
    {
        LastSyncedAt = now;
        UpdatedAt = now;
    }

    public void SoftDelete(DateTimeOffset now)
    {
        IsDeleted = true;
        DeletedAt = now;
        UpdatedAt = now;
    }
}
