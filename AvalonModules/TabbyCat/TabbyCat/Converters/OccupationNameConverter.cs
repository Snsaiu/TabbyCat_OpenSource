using System.Globalization;
using Avalonia.Data;
using TabbyCat.Repository.Entities.AiEntities;
using TabbyCat.Shared.Enums;
using TuDog.Extensions;

namespace TabbyCat.Converters;

public class OccupationNameConverter : ValueConvertBase
{
    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not AiChatSessionEntity session) return string.Empty;
        if (session.Occupation == AssistantOccupation.Custom)
            return session.CustomOccupationName;
        return session.Occupation.ToString(); // tudo:这里需要本地化
    }
}