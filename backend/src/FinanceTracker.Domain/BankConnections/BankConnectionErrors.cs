using FinanceTracker.SharedKernel;

namespace FinanceTracker.Domain.BankConnections;

public static class BankConnectionErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("BankConnections.NotFound", $"Bank connection with id '{id}' was not found.");

    public static Error AlreadyLinked(string institutionId) =>
        Error.Conflict("BankConnections.AlreadyLinked", $"A connection to '{institutionId}' already exists.");

    public static Error Expired(Guid id) =>
        Error.Problem("BankConnections.Expired", $"Bank connection '{id}' has expired. Please re-authorize.");
}
