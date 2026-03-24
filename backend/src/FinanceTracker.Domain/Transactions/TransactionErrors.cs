using FinanceTracker.SharedKernel;

namespace FinanceTracker.Domain.Transactions;

public static class TransactionErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Transactions.NotFound", $"Transaction with id '{id}' was not found.");

    public static Error AlreadyExists(string externalId) =>
        Error.Conflict("Transactions.AlreadyExists", $"Transaction with external id '{externalId}' already exists.");
}
