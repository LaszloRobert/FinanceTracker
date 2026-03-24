using FinanceTracker.Application.Abstractions.Data;
using FinanceTracker.Domain.Accounts;
using FinanceTracker.Domain.BankConnections;
using FinanceTracker.Domain.Categories;
using FinanceTracker.Domain.Transactions;
using FinanceTracker.Domain.Users;
using FinanceTracker.Infrastructure.DomainEvents;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure.Database;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    IDomainEventsDispatcher domainEventsDispatcher)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<User> Users { get; set; }

    public DbSet<BankConnection> BankConnections { get; set; }

    public DbSet<Account> Accounts { get; set; }

    public DbSet<Transaction> Transactions { get; set; }

    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        modelBuilder.HasDefaultSchema(Schemas.Default);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        int result = await base.SaveChangesAsync(cancellationToken);

        await PublishDomainEventsAsync(cancellationToken);

        return result;
    }

    private async Task PublishDomainEventsAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            var domainEvents = ChangeTracker
                .Entries<SharedKernel.Entity>()
                .Select(entry => entry.Entity)
                .SelectMany(entity =>
                {
                    var events = entity.DomainEvents.ToList();
                    entity.ClearDomainEvents();
                    return events;
                })
                .ToList();

            if (domainEvents.Count == 0)
            {
                break;
            }

            await domainEventsDispatcher.DispatchAsync(domainEvents, cancellationToken);
        }
    }
}
