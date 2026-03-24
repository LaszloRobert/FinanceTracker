using FinanceTracker.Api.Infrastructure;
using FinanceTracker.Application.Abstractions.Messaging;
using FinanceTracker.Application.Accounts.GetAll;
using FinanceTracker.Application.Accounts.UpdateDisplayName;
using FinanceTracker.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Api.Controllers;

[ApiController]
[Route("api/accounts")]
[Authorize]
public sealed class AccountsController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromServices] IQueryHandler<GetAccountsQuery, List<AccountResponse>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetAccountsQuery();

        Result<List<AccountResponse>> result = await handler.Handle(query, cancellationToken);

        return result.Match(Ok, CustomResults.Problem);
    }

    [HttpPut("{id:guid}/display-name")]
    public async Task<IActionResult> UpdateDisplayName(
        Guid id,
        [FromBody] UpdateDisplayNameRequest request,
        [FromServices] ICommandHandler<UpdateDisplayNameCommand> handler,
        CancellationToken cancellationToken)
    {
        var command = new UpdateDisplayNameCommand(id, request.DisplayName);

        Result result = await handler.Handle(command, cancellationToken);

        return result.IsSuccess ? NoContent() : CustomResults.Problem(result.Error);
    }
}

public sealed record UpdateDisplayNameRequest(string DisplayName);
