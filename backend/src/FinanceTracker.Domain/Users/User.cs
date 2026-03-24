using FinanceTracker.SharedKernel;

namespace FinanceTracker.Domain.Users;

public sealed class User : Entity
{
    public string Email { get; private set; } = default!;
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;

    private User() { }

    public static User Create(string email, string firstName, string lastName, string passwordHash, DateTime now)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            PasswordHash = passwordHash,
            CreatedAt = now
        };

        user.Raise(new UserRegisteredDomainEvent(user.Id));
        return user;
    }
}
