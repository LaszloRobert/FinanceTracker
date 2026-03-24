using FinanceTracker.Application.Abstractions.Messaging;

namespace FinanceTracker.Application.Categories.Update;

public sealed record UpdateCategoryCommand(
    Guid CategoryId,
    string Name,
    string? Icon,
    string? Color,
    Guid? ParentCategoryId) : ICommand;
