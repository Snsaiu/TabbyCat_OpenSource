namespace TuDog.IocAutoRegisterSourceGenerator.Templates;

public static class SourceGeneratorHelper
{
    public const string Attribute = @"
namespace NetEscapades.EnumGenerators
{
    [System.AttributeUsage(System.AttributeTargets.Enum)]
    public class EnumExtensionsAttribute : System.Attribute
    {
    }
}";
}