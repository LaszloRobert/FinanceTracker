using FinanceTracker.Application.Abstractions.Authentication;
using FinanceTracker.Application.Abstractions.Data;
using FinanceTracker.Application.Abstractions.Messaging;
using FinanceTracker.Domain.Users;
using FinanceTracker.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Application.Users.Register;

internal sealed class RegisterUserCommandHandler(
    IApplicationDbContext context,
    IPasswordHasher passwordHasher,
    TimeProvider timeProvider)
    : ICommandHandler<RegisterUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        #pragma warning disable CA1308 // Emails are conventionally normalized to lowercase
        string normalizedEmail = command.Email.Trim().ToLowerInvariant();
#pragma warning restore CA1308

        bool emailExists = await context.Users
            .AnyAsync(u => u.Email == normalizedEmail, cancellationToken);

        if (emailExists)
        {
            return Result.Failure<Guid>(UserErrors.EmailAlreadyExists());
        }

        string passwordHash = passwordHasher.Hash(command.Password);

        DateTimeOffset now = timeProvider.GetUtcNow();

        var user = User.Create(normalizedEmail, command.FirstName, command.LastName, passwordHash, now);

        context.Users.Add(user);

        await context.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
