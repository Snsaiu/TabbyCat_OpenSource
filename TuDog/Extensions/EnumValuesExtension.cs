using Avalonia.Markup.Xaml;
using Avalonia.Metadata;

namespace TuDog.Extensions;

public sealed class EnumValuesExtension : MarkupExtension
{
    [ConstructorArgument("enumType")] public Type EnumType { get; set; } = null!;

    /// <summary>
    /// 排除，使用|分隔
    /// </summary>
    public string? Exclude { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (EnumType is null || !EnumType.IsEnum) throw new ArgumentException("EnumType must be a valid Enum type");

        var values = EnumType.GetEnumValues();
        var enumValues = values.Cast<object>().ToList();
        var excludeValues = Exclude?.Split('|');

        if (excludeValues is null)
            return enumValues;

        List<object> result = new();
        foreach (var item in enumValues)
        {
            if (excludeValues.Contains(item.ToString())) continue;
            result.Add(item);
        }

        return result;
    }
}