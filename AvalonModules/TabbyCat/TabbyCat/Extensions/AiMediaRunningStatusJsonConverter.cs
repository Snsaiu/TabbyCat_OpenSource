using Newtonsoft.Json.Converters;
using TabbyCat.Enums;
using TabbyCat.Shared.Enums;

namespace TabbyCat.Extensions;

public sealed class AiMediaRunningStatusJsonConverter : StringEnumConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        writer.WriteValue(value?.ToString().ToLower());
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue,
        JsonSerializer serializer)
    {
        if (reader.Value is not string role) throw new JsonSerializationException("Expected role");

        return role.ToLower() switch
        {
            "pending" => AiMediaRunningStatus.Pending,
            "running" => AiMediaRunningStatus.Running,
            "suspended" => AiMediaRunningStatus.Suspended,
            "succeeded" => AiMediaRunningStatus.Success,
            "failed" => AiMediaRunningStatus.Failed,
            "unknown" => AiMediaRunningStatus.Unknown,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}