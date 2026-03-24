using FinanceTracker.Domain.Accounts;
using FinanceTracker.Domain.BankConnections;
using FinanceTracker.Domain.Categories;
using FinanceTracker.Domain.Transactions;
using FinanceTracker.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Application.Abstractions.Data;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }

    DbSet<BankConnection> BankConnections { get; }

    DbSet<Account> Accounts { get; }

    DbSet<Transaction> Transactions { get; }

    DbSet<Category> Categories { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
