using FinanceTracker.Application.Abstractions.Messaging;

namespace FinanceTracker.Application.Categories.Delete;

public sealed record DeleteCategoryCommand(Guid CategoryId) : ICommand;
