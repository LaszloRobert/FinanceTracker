using FinanceTracker.Api.Infrastructure;
using FinanceTracker.SharedKernel;
using FinanceTracker.Application.Abstractions.Messaging;
using FinanceTracker.Application.Users.Login;
using FinanceTracker.Application.Users.Register;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(
        RegisterUserRequest request,
        [FromServices] ICommandHandler<RegisterUserCommand, Guid> handler,
        CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(
            request.Email,
            request.FirstName,
            request.LastName,
            request.Password);

        Result<Guid> result = await handler.Handle(command, cancellationToken);

        return result.Match(
            userId => StatusCode(StatusCodes.Status201Created, new { userId }),
            error => CustomResults.Problem(error));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(
        LoginUserRequest request,
        [FromServices] ICommandHandler<LoginUserCommand, string> handler,
        CancellationToken cancellationToken)
    {
        var command = new LoginUserCommand(request.Email, request.Password);

        Result<string> result = await handler.Handle(command, cancellationToken);

        return result.Match(
            token => Ok(new { token }),
            error => CustomResults.Problem(error));
    }
}

public sealed record RegisterUserRequest(string Email, string FirstName, string LastName, string Password);

public sealed record LoginUserRequest(string Email, string Password);
