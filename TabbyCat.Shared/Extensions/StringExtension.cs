using System.Text;
using Newtonsoft.Json;

namespace TabbyCat.Shared.Extensions;

public static class StringExtension
{
    public static byte[] ToBytes(this string value)
    {
        return Encoding.UTF8.GetBytes(value);
    }

    public static T ToObject<T>(this string value)
    {
        return JsonConvert.DeserializeObject<T>(value) ?? throw new InvalidCastException();
    }

    public static string ToJson(this object value)
    {
        return JsonConvert.SerializeObject(value);
    }
    
    public static string Truncate(this string value, int length)
    {
        return value.Length <= length ? value : value.Substring(0, length) + "...";
    }

}