using Microsoft.Extensions.Logging;

using Serilog;
using Serilog.Sinks.Grafana.Loki;

namespace Microsoft.Extensions.DependencyInjection;

public static class LoggerMetricsBuilder
{
    public static void AddLoggerBuilder(this IServiceCollection services, string lokiUrl, IEnumerable<LokiLabel>? lokiLabels)
    {
        var logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .WriteTo.GrafanaLoki(lokiUrl, lokiLabels)

            .CreateLogger();

        services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddSerilog(logger);

        });
    }

}