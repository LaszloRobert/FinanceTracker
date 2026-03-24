using FinanceTracker.SharedKernel;

namespace FinanceTracker.Domain.BankConnections;

public sealed class BankConnection : Entity
{
    public Guid UserId { get; private set; }
    public string InstitutionId { get; private set; } = default!;
    public string InstitutionName { get; private set; } = default!;
    public string? InstitutionLogo { get; private set; }
    public string RequisitionId { get; private set; } = default!;
    public BankConnectionStatus Status { get; private set; }
    public DateTime LinkedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? LastConsentRenewedAt { get; private set; }
    public int AccessValidForDays { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    private BankConnection() { }

    public static BankConnection Create(
        Guid userId,
        string institutionId,
        string institutionName,
        string? institutionLogo,
        string requisitionId,
        int accessValidForDays,
        DateTime now)
    {
        var connection = new BankConnection
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            InstitutionId = institutionId,
            InstitutionName = institutionName,
            InstitutionLogo = institutionLogo,
            RequisitionId = requisitionId,
            Status = BankConnectionStatus.Linked,
            LinkedAt = now,
            LastConsentRenewedAt = now,
            AccessValidForDays = accessValidForDays,
            ExpiresAt = now.AddDays(accessValidForDays),
            CreatedAt = now
        };

        connection.Raise(new BankConnectionLinkedDomainEvent(connection.Id));
        return connection;
    }

    public void MarkExpired(DateTime now)
    {
        if (Status == BankConnectionStatus.Expired)
        {
            return;
        }

        Status = BankConnectionStatus.Expired;
        UpdatedAt = now;
        Raise(new BankConnectionExpiredDomainEvent(Id));
    }

    public void MarkError(DateTime now)
    {
        Status = BankConnectionStatus.Error;
        UpdatedAt = now;
        Raise(new BankConnectionErrorDomainEvent(Id));
    }

    public void RenewConsent(string newRequisitionId, DateTime now)
    {
        RequisitionId = newRequisitionId;
        Status = BankConnectionStatus.Linked;
        LastConsentRenewedAt = now;
        ExpiresAt = now.AddDays(AccessValidForDays);
        UpdatedAt = now;
    }

    public void Revoke(DateTime now)
    {
        Status = BankConnectionStatus.Revoked;
        UpdatedAt = now;
    }

    public void SoftDelete(DateTime now)
    {
        IsDeleted = true;
        DeletedAt = now;
        UpdatedAt = now;
    }
}
