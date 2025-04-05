using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using Serilog;
using Serilog.Sinks.Loki;
using Serilog.Sinks.Loki.Labels;

namespace Microsoft.Extensions.DependencyInjection;

public static class LoggerMetricsBuilder
{

    public static OpenTelemetryBuilder AddOpenTelemetryBuilder(this IServiceCollection services,string meterName)
    {
        services.AddOpenTelemetry().ConfigureResource(build => build.AddDetector(sp => new SystemResourceDetector()))
            .WithMetrics(builder => builder.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(meterName)).AddMeter($"{meterName}.Metrics"));

        throw new NotImplementedException();
    }

    public static void AddLoggerBuilder(this IServiceCollection services,string lokiUrl,ILogLabelProvider? logLabelProvider)
    {
        var logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .WriteTo.
            LokiHttp(()=>new LokiSinkConfiguration(){ LokiUrl = lokiUrl, 
                LogLabelProvider = logLabelProvider})
            .CreateLogger();

        services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddSerilog(logger);
        });
    }
    
}