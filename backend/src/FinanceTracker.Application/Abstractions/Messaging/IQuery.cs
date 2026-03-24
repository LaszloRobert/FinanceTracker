namespace FinanceTracker.Application.Abstractions.Messaging;

public interface IQueryBase;

public interface IQuery<TResponse> : IQueryBase;
