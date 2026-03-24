using FinanceTracker.Api.Infrastructure;
using FinanceTracker.Application.Abstractions.Messaging;
using FinanceTracker.Application.Categories.Create;
using FinanceTracker.Application.Categories.Delete;
using FinanceTracker.Application.Categories.GetAll;
using FinanceTracker.Application.Categories.Update;
using FinanceTracker.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Api.Controllers;

[ApiController]
[Route("api/categories")]
[Authorize]
public sealed class CategoriesController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromServices] IQueryHandler<GetCategoriesQuery, List<CategoryResponse>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetCategoriesQuery();

        Result<List<CategoryResponse>> result = await handler.Handle(query, cancellationToken);

        return result.Match(
            categories => Ok(categories),
            error => CustomResults.Problem(error));
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateCategoryRequest request,
        [FromServices] ICommandHandler<CreateCategoryCommand, Guid> handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateCategoryCommand(
            request.Name,
            request.Icon,
            request.Color,
            request.ParentCategoryId);

        Result<Guid> result = await handler.Handle(command, cancellationToken);

        return result.Match(
            categoryId => StatusCode(StatusCodes.Status201Created, new { categoryId }),
            error => CustomResults.Problem(error));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateCategoryRequest request,
        [FromServices] ICommandHandler<UpdateCategoryCommand> handler,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCategoryCommand(
            id,
            request.Name,
            request.Icon,
            request.Color,
            request.ParentCategoryId);

        Result result = await handler.Handle(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : CustomResults.Problem(result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        [FromServices] ICommandHandler<DeleteCategoryCommand> handler,
        CancellationToken cancellationToken)
    {
        var command = new DeleteCategoryCommand(id);

        Result result = await handler.Handle(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : CustomResults.Problem(result.Error);
    }
}

public sealed record CreateCategoryRequest(string Name, string? Icon, string? Color, Guid? ParentCategoryId);

public sealed record UpdateCategoryRequest(string Name, string? Icon, string? Color, Guid? ParentCategoryId);
