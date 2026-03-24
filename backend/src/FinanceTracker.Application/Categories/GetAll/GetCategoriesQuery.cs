using FinanceTracker.Application.Abstractions.Messaging;

namespace FinanceTracker.Application.Categories.GetAll;

public sealed record GetCategoriesQuery : IQuery<List<CategoryResponse>>;

public sealed record CategoryResponse(
    Guid Id,
    string Name,
    string? Icon,
    string? Color,
    Guid? ParentCategoryId,
    bool IsDefault);
