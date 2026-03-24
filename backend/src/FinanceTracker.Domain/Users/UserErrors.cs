using FinanceTracker.SharedKernel;

namespace FinanceTracker.Domain.Users;

public static class UserErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Users.NotFound", $"User with id '{id}' was not found.");

    public static Error EmailAlreadyExists() =>
        Error.Conflict("Users.EmailExists", "A user with this email already exists.");

    public static Error Unauthorized() =>
        Error.Unauthorized("Users.Unauthorized", "You are not authorized to perform this action.");

    public static Error InvalidCredentials() =>
        Error.Unauthorized("Users.InvalidCredentials", "The provided credentials are invalid.");
}
