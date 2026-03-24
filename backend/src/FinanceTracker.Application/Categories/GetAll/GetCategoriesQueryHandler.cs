using FinanceTracker.Application.Abstractions.Authentication;
using FinanceTracker.Application.Abstractions.Data;
using FinanceTracker.Application.Abstractions.Messaging;
using FinanceTracker.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Application.Categories.GetAll;

internal sealed class GetCategoriesQueryHandler(
    IApplicationDbContext context,
    IUserContext userContext)
    : IQueryHandler<GetCategoriesQuery, List<CategoryResponse>>
{
    public async Task<Result<List<CategoryResponse>>> Handle(GetCategoriesQuery query, CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;

        List<CategoryResponse> categories = await context.Categories
            .Where(c => c.UserId == userId || c.IsDefault)
            .Select(c => new CategoryResponse(
                c.Id,
                c.Name,
                c.Icon,
                c.Color,
                c.ParentCategoryId,
                c.IsDefault))
            .ToListAsync(cancellationToken);

        return categories;
    }
}
