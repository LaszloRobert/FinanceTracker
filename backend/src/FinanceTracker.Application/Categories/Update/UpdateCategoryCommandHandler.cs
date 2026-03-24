using FinanceTracker.Application.Abstractions.Authentication;
using FinanceTracker.Application.Abstractions.Data;
using FinanceTracker.Application.Abstractions.Messaging;
using FinanceTracker.Domain.Categories;
using FinanceTracker.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Application.Categories.Update;

internal sealed class UpdateCategoryCommandHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    TimeProvider timeProvider)
    : ICommandHandler<UpdateCategoryCommand>
{
    public async Task<Result> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;

        Category? category = await context.Categories
            .FirstOrDefaultAsync(c => c.Id == command.CategoryId && c.UserId == userId, cancellationToken);

        if (category is null)
        {
            return Result.Failure(CategoryErrors.NotFound(command.CategoryId));
        }

        DateTimeOffset now = timeProvider.GetUtcNow();

        category.Update(command.Name.Trim(), command.Icon, command.Color, command.ParentCategoryId, now);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
