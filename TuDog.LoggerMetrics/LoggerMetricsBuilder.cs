using Microsoft.Extensions.Logging;

using Serilog;
using Serilog.Sinks.Grafana.Loki;
using Serilog.Sinks.SystemConsole.Themes;

namespace Microsoft.Extensions.DependencyInjection;

public static class LoggerMetricsBuilder
{
    public static void AddLoggerBuilder(this IServiceCollection services, string lokiUrl, IEnumerable<LokiLabel>? lokiLabels,string appName)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),"YouYan", appName, "Logs");
        Directory.CreateDirectory(logDir);

        var logPath = Path.Combine(logDir, "log-.txt");
        
        var logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.Console(theme: AnsiConsoleTheme.Code)
            .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
            .WriteTo.GrafanaLoki(lokiUrl, lokiLabels)
            .CreateLogger();

        services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddSerilog(logger);

        });

    }

}