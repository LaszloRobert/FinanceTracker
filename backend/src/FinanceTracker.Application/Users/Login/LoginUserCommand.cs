using FinanceTracker.Application.Abstractions.Messaging;

namespace FinanceTracker.Application.Users.Login;

public sealed record LoginUserCommand(string Email, string Password) : ICommand<string>;
