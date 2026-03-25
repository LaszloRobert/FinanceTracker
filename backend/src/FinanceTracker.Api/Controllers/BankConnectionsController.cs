using FinanceTracker.Api.Infrastructure;
using FinanceTracker.Application.Abstractions.Messaging;
using FinanceTracker.Application.BankConnections.Callback;
using FinanceTracker.Application.BankConnections.Connect;
using FinanceTracker.Application.BankConnections.Delete;
using FinanceTracker.Application.BankConnections.GetAll;
using FinanceTracker.Application.BankConnections.GetInstitutions;
using FinanceTracker.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Api.Controllers;

[ApiController]
[Route("api/bank-connections")]
[Authorize]
public sealed class BankConnectionsController : ControllerBase
{
    [HttpGet("institutions")]
    public async Task<IActionResult> GetInstitutions(
        [FromQuery] [System.ComponentModel.DataAnnotations.Required] [System.ComponentModel.DataAnnotations.StringLength(2, MinimumLength = 2)] string countryCode,
        [FromServices] IQueryHandler<GetInstitutionsQuery, List<InstitutionResponse>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetInstitutionsQuery(countryCode);

        Result<List<InstitutionResponse>> result = await handler.Handle(query, cancellationToken);

        return result.Match(Ok, CustomResults.Problem);
    }

    [HttpPost("connect")]
    public async Task<IActionResult> Connect(
        [FromBody] ConnectBankRequest request,
        [FromServices] ICommandHandler<ConnectBankCommand, ConnectBankResponse> handler,
        CancellationToken cancellationToken)
    {
        var command = new ConnectBankCommand(request.InstitutionId, request.RedirectUrl);

        Result<ConnectBankResponse> result = await handler.Handle(command, cancellationToken);

        return result.Match(Ok, CustomResults.Problem);
    }

    [HttpPost("callback")]
    public async Task<IActionResult> Callback(
        [FromBody] BankCallbackRequest request,
        [FromServices] ICommandHandler<ProcessBankCallbackCommand, Guid> handler,
        CancellationToken cancellationToken)
    {
        var command = new ProcessBankCallbackCommand(request.RequisitionId);

        Result<Guid> result = await handler.Handle(command, cancellationToken);

        return result.Match(
            bankConnectionId => Ok(new { bankConnectionId }),
            CustomResults.Problem);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromServices] IQueryHandler<GetBankConnectionsQuery, List<BankConnectionResponse>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetBankConnectionsQuery();

        Result<List<BankConnectionResponse>> result = await handler.Handle(query, cancellationToken);

        return result.Match(Ok, CustomResults.Problem);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        [FromServices] ICommandHandler<DeleteBankConnectionCommand> handler,
        CancellationToken cancellationToken)
    {
        var command = new DeleteBankConnectionCommand(id);

        Result result = await handler.Handle(command, cancellationToken);

        return result.IsSuccess ? NoContent() : CustomResults.Problem(result.Error);
    }
}

public sealed record ConnectBankRequest(string InstitutionId, Uri RedirectUrl);

public sealed record BankCallbackRequest(string RequisitionId);
