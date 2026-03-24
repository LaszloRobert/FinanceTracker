using FinanceTracker.Application.Abstractions.Authentication;
using FinanceTracker.Application.Abstractions.Data;
using FinanceTracker.Application.Abstractions.Messaging;
using FinanceTracker.Domain.Categories;
using FinanceTracker.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Application.Categories.Create;

internal sealed class CreateCategoryCommandHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    TimeProvider timeProvider)
    : ICommandHandler<CreateCategoryCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;

        string trimmedName = command.Name.Trim();

        bool nameExists = await context.Categories
            .AnyAsync(c => c.UserId == userId && EF.Functions.Like(c.Name, trimmedName), cancellationToken);

        if (nameExists)
        {
            return Result.Failure<Guid>(CategoryErrors.NameAlreadyExists(command.Name));
        }

        DateTimeOffset now = timeProvider.GetUtcNow();

        var category = Category.Create(
            userId,
            command.Name.Trim(),
            command.Icon,
            command.Color,
            command.ParentCategoryId,
            isDefault: false,
            now);

        context.Categories.Add(category);

        await context.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}
