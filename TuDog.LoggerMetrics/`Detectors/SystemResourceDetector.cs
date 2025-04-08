//using System.Runtime.InteropServices;
//using OpenTelemetry.Resources;

//namespace Microsoft.Extensions.DependencyInjection;

//public sealed class SystemResourceDetector:IResourceDetector
//{
//    public Resource Detect()
//    {
//        var attributes = new List<KeyValuePair<string, object>>
//        {
//            new KeyValuePair<string, object>("CPU", RuntimeInformation.ProcessArchitecture),
//            new KeyValuePair<string, object>("OS", RuntimeInformation.OSDescription),
//            new KeyValuePair<string, object>("Architecture", RuntimeInformation.OSArchitecture),
//        };
//        return new Resource(attributes);
//    }
//}