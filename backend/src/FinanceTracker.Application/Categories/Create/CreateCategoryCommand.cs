using FinanceTracker.Application.Abstractions.Messaging;

namespace FinanceTracker.Application.Categories.Create;

public sealed record CreateCategoryCommand(
    string Name,
    string? Icon,
    string? Color,
    Guid? ParentCategoryId) : ICommand<Guid>;
