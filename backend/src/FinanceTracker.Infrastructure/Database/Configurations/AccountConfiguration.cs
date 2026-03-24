using FinanceTracker.Domain.Accounts;
using FinanceTracker.Domain.BankConnections;
using FinanceTracker.Domain.Users;
using FinanceTracker.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceTracker.Infrastructure.Database.Configurations;

internal sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.ExternalId)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(a => a.Iban)
            .HasMaxLength(34);

        builder.Property(a => a.Currency)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(a => a.OwnerName)
            .HasMaxLength(256);

        builder.Property(a => a.DisplayName)
            .HasMaxLength(128);

        builder.Property(a => a.Product)
            .HasMaxLength(64);

        builder.OwnsMany(a => a.Balances, balance =>
        {
            balance.ToJson("balances");
        });

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<BankConnection>()
            .WithMany()
            .HasForeignKey(a => a.BankConnectionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => a.UserId);
        builder.HasIndex(a => a.BankConnectionId);

        builder.HasIndex(a => a.ExternalId)
            .IsUnique();

        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}
