using System.IdentityModel.Tokens.Jwt;
using FinanceTracker.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Http;

namespace FinanceTracker.Infrastructure.Authentication;

internal sealed class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public Guid UserId =>
        httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value is { } userId
            ? Guid.Parse(userId)
            : throw new InvalidOperationException("User context is not available.");
}
