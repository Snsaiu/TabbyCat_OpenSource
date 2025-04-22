using Newtonsoft.Json.Linq;
using TabbyCat.Models.Appendix;

namespace TabbyCat.Extensions;

public sealed class IAppendixModelJsonConverter : JsonConverter<IEnumerable<IAiAppendixModel>>
{
    public override void WriteJson(JsonWriter writer, IEnumerable<IAiAppendixModel>? value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }

    public override IEnumerable<IAiAppendixModel>? ReadJson(JsonReader reader, Type objectType,
        IEnumerable<IAiAppendixModel>? existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        var jsonObject = JObject.Load(reader);


        throw new NotSupportedException("Unknown type");
    }
}