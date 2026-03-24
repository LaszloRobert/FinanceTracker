using FinanceTracker.Domain.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceTracker.Infrastructure.Database.Configurations;

internal sealed class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.ExternalId)
            .HasMaxLength(256);

        builder.Property(t => t.InternalId)
            .HasMaxLength(256);

        builder.OwnsOne(t => t.TransactionAmount, amount =>
        {
            amount.Property(a => a.Amount)
                .HasColumnName("amount")
                .HasPrecision(18, 4)
                .IsRequired();

            amount.Property(a => a.Currency)
                .HasColumnName("currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(t => t.CreditorName).HasMaxLength(256);
        builder.Property(t => t.CreditorIban).HasMaxLength(34);
        builder.Property(t => t.DebtorName).HasMaxLength(256);
        builder.Property(t => t.DebtorIban).HasMaxLength(34);
        builder.Property(t => t.RemittanceInfo).HasMaxLength(1024);
        builder.Property(t => t.AdditionalInfo).HasMaxLength(1024);
        builder.Property(t => t.MerchantCategoryCode).HasMaxLength(4);
        builder.Property(t => t.BankTransactionCode).HasMaxLength(64);

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(16)
            .IsRequired();

        builder.HasIndex(t => t.UserId);
        builder.HasIndex(t => t.AccountId);
        builder.HasIndex(t => t.BookingDate);
        builder.HasIndex(t => t.CategoryId);

        builder.HasIndex(t => new { t.AccountId, t.ExternalId })
            .IsUnique()
            .HasFilter("external_id IS NOT NULL");

        builder.HasIndex(t => new { t.AccountId, t.InternalId })
            .IsUnique()
            .HasFilter("internal_id IS NOT NULL");
    }
}
