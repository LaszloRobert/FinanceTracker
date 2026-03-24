using FinanceTracker.SharedKernel;

namespace FinanceTracker.Domain.Accounts;

public static class AccountErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Accounts.NotFound", $"Account with id '{id}' was not found.");

    public static Error ExternalIdAlreadyExists(string externalId) =>
        Error.Conflict("Accounts.ExternalIdExists", $"Account with external id '{externalId}' already exists.");
}
