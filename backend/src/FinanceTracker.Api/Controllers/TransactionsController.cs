using FinanceTracker.Api.Infrastructure;
using FinanceTracker.Application.Abstractions.Messaging;
using FinanceTracker.Application.Transactions.Categorize;
using FinanceTracker.Application.Transactions.GetAll;
using FinanceTracker.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Api.Controllers;

[ApiController]
[Route("api/transactions")]
[Authorize]
public sealed class TransactionsController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? accountId,
        [FromQuery] Guid? categoryId,
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromServices] IQueryHandler<GetTransactionsQuery, TransactionsPageResponse>? handler = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTransactionsQuery(accountId, categoryId, dateFrom, dateTo, page, pageSize);

        Result<TransactionsPageResponse> result = await handler!.Handle(query, cancellationToken);

        return result.Match(Ok, CustomResults.Problem);
    }

    [HttpPut("{id:guid}/category")]
    public async Task<IActionResult> Categorize(
        Guid id,
        [FromBody] CategorizeTransactionRequest request,
        [FromServices] ICommandHandler<CategorizeTransactionCommand>? handler = null,
        CancellationToken cancellationToken = default)
    {
        var command = new CategorizeTransactionCommand(id, request.CategoryId);

        Result result = await handler!.Handle(command, cancellationToken);

        return result.IsSuccess ? NoContent() : CustomResults.Problem(result.Error);
    }
}

public sealed record CategorizeTransactionRequest(Guid? CategoryId);
