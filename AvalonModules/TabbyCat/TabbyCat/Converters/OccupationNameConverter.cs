using System.Globalization;
using Avalonia.Data;
using TabbyCat.Repository.Entities.AiEntities;
using TabbyCat.Shared;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Languages;
using TuDog.Extensions;

namespace TabbyCat.Converters;

public class OccupationNameConverter : ValueConvertBase
{
    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not AiChatSessionEntity session) return string.Empty;
        
        return session.Occupation == AssistantOccupation.Custom ? session.CustomOccupationName : LocalizationResourceManager.Instance[session.Occupation.ToString()];
    }
}