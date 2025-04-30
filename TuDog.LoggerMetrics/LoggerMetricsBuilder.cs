using Microsoft.Extensions.Logging;

using Serilog;
using Serilog.Sinks.Grafana.Loki;
using Serilog.Sinks.SystemConsole.Themes;

namespace Microsoft.Extensions.DependencyInjection;

public static class LoggerMetricsBuilder
{
    public static void AddLoggerBuilder(this IServiceCollection services, string lokiUrl, IEnumerable<LokiLabel>? lokiLabels)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.Console(theme: AnsiConsoleTheme.Code)
            .WriteTo.GrafanaLoki(lokiUrl, lokiLabels)
            .CreateLogger();

        services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddSerilog(logger);

        });

    }

}