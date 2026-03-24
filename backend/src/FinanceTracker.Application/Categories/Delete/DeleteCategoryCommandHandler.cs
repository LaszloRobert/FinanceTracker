using FinanceTracker.Application.Abstractions.Authentication;
using FinanceTracker.Application.Abstractions.Data;
using FinanceTracker.Application.Abstractions.Messaging;
using FinanceTracker.Domain.Categories;
using FinanceTracker.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Application.Categories.Delete;

internal sealed class DeleteCategoryCommandHandler(
    IApplicationDbContext context,
    IUserContext userContext)
    : ICommandHandler<DeleteCategoryCommand>
{
    public async Task<Result> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;

        Category? category = await context.Categories
            .FirstOrDefaultAsync(c => c.Id == command.CategoryId && c.UserId == userId, cancellationToken);

        if (category is null)
        {
            return Result.Failure(CategoryErrors.NotFound(command.CategoryId));
        }

        if (category.IsDefault)
        {
            return Result.Failure(CategoryErrors.CannotDeleteDefault());
        }

        context.Categories.Remove(category);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
