using FinanceTracker.Infrastructure.DomainEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FinanceTracker.Infrastructure.Database;

internal sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../FinanceTracker.Api"))
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder
            .UseNpgsql(configuration.GetConnectionString("Database"))
            .UseSnakeCaseNamingConvention();

        var noOpDispatcher = new NoOpDomainEventsDispatcher();

        return new ApplicationDbContext(optionsBuilder.Options, noOpDispatcher);
    }

    private sealed class NoOpDomainEventsDispatcher : IDomainEventsDispatcher
    {
        public Task DispatchAsync(
            IEnumerable<SharedKernel.IDomainEvent> domainEvents,
            CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
