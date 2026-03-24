using FinanceTracker.SharedKernel;

namespace FinanceTracker.Domain.Categories;

public sealed class Category : Entity
{
    public Guid UserId { get; private set; }
    public string Name { get; private set; } = default!;
    public string? Icon { get; private set; }
    public string? Color { get; private set; }
    public Guid? ParentCategoryId { get; private set; }
    public bool IsDefault { get; private set; }

    private Category() { }

    public static Category Create(
        Guid userId,
        string name,
        string? icon,
        string? color,
        Guid? parentCategoryId,
        bool isDefault,
        DateTime now)
    {
        return new Category
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = name,
            Icon = icon,
            Color = color,
            ParentCategoryId = parentCategoryId,
            IsDefault = isDefault,
            CreatedAt = now
        };
    }

    public static Category CreateDefault(string name, string? icon, string? color, DateTime now)
    {
        return Create(Guid.Empty, name, icon, color, null, true, now);
    }

    public void Update(string name, string? icon, string? color, Guid? parentCategoryId, DateTime now)
    {
        Name = name;
        Icon = icon;
        Color = color;
        ParentCategoryId = parentCategoryId;
        UpdatedAt = now;
    }
}
