using FinanceTracker.Domain.Users;

namespace FinanceTracker.Application.Abstractions.Authentication;

public interface ITokenProvider
{
    string GenerateToken(User user);
}
