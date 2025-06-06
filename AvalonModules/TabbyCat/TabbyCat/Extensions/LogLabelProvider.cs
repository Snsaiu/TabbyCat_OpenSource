using Serilog.Sinks.Grafana.Loki;

using System.IO;
using System.Runtime.InteropServices;

namespace TabbyCat.Extensions;


public class LogLabelProvider()
{

    public static IEnumerable<LokiLabel> GetLabels()
    {
        var labels = new List<LokiLabel>();

        labels.Add(new LokiLabel { Key = "AppName", Value = "TabbyCat" });

        var vFile = Path.Join(Environment.ProcessPath, "v");
        if (File.Exists(vFile))
            labels.Add(new LokiLabel { Key = "Version", Value = File.ReadAllText(vFile) });

        labels.Add(new LokiLabel { Key = "CPU", Value = RuntimeInformation.ProcessArchitecture.ToString() });
        labels.Add(new LokiLabel { Key = "OS", Value = RuntimeInformation.OSDescription });
        labels.Add(new LokiLabel { Key = "Architecture", Value = RuntimeInformation.OSArchitecture.ToString() });

        return labels;
    }


}
