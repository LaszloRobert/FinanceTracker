using FinanceTracker.Application.Abstractions.Messaging;

namespace FinanceTracker.Application.Users.Register;

public sealed record RegisterUserCommand(
    string Email,
    string FirstName,
    string LastName,
    string Password) : ICommand<Guid>;
