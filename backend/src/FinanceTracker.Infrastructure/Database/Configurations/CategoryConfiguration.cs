using FinanceTracker.Domain.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceTracker.Infrastructure.Database.Configurations;

internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(c => c.Icon)
            .HasMaxLength(64);

        builder.Property(c => c.Color)
            .HasMaxLength(16);

        builder.HasIndex(c => c.UserId);

        builder.HasIndex(c => new { c.UserId, c.Name })
            .IsUnique();
    }
}
