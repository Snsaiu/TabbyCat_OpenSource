using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Events;
using Serilog.Sinks.Loki;
using Serilog.Sinks.Loki.Labels;
using TabbyCat.IServices;
using TuDog.Bootstrap;

namespace TabbyCat.Extensions;


public class LogLabelProvider() : ILogLabelProvider
{
   
    public IList<LokiLabel> GetLabels()
    {
        var labels = new List<LokiLabel>();

        var user = TuDogApplication.ServiceProvider.GetRequiredService<IUser>();
        
        labels.Add(new LokiLabel("AppName","TabbyCat"));
        
        if (user.LoginSuccess())
        {
           labels.Add(new LokiLabel("User_Email",user.Email));
        }

        var vFile = Path.Join(Environment.ProcessPath, "v");
        if(File.Exists(vFile))
            labels.Add(new LokiLabel("Version",File.ReadAllText(vFile)));
        
        labels.Add(new LokiLabel("CPU",RuntimeInformation.ProcessArchitecture.ToString()));
        labels.Add(new LokiLabel("OS",RuntimeInformation.OSDescription));
        labels.Add(new LokiLabel("Architecture",RuntimeInformation.OSArchitecture.ToString()));

        return labels;
    }

    public IList<string> PropertiesAsLabels { get; } = [];
    public IList<string> PropertiesToAppend { get; } = [];

    public LokiFormatterStrategy FormatterStrategy { get; } =
        LokiFormatterStrategy.SpecificPropertiesAsLabelsAndRestAppended;
}
