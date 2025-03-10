using System;

namespace TuDog.IocAttribute
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class IconBuildAttribute : Attribute
    {
        public IconBuildAttribute(string jsonUrl) { }
    }
}
