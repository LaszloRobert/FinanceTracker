using FinanceTracker.Domain.BankConnections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceTracker.Infrastructure.Database.Configurations;

internal sealed class BankConnectionConfiguration : IEntityTypeConfiguration<BankConnection>
{
    public void Configure(EntityTypeBuilder<BankConnection> builder)
    {
        builder.HasKey(bc => bc.Id);

        builder.Property(bc => bc.InstitutionId)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(bc => bc.InstitutionName)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(bc => bc.InstitutionLogo)
            .HasMaxLength(512);

        builder.Property(bc => bc.RequisitionId)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(bc => bc.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.HasIndex(bc => bc.UserId);

        builder.HasIndex(bc => bc.RequisitionId)
            .IsUnique();

        builder.HasQueryFilter(bc => !bc.IsDeleted);
    }
}
