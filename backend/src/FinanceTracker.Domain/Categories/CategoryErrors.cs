using FinanceTracker.SharedKernel;

namespace FinanceTracker.Domain.Categories;

public static class CategoryErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Categories.NotFound", $"Category with id '{id}' was not found.");

    public static Error NameAlreadyExists(string name) =>
        Error.Conflict("Categories.NameExists", $"Category with name '{name}' already exists.");
}
