using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TuDog.Bootstrap;

namespace TabbyCat.Extensions;

public static class Logger
{
    private static ILogger _logger;

    static Logger()
    {
        var factory = TuDogApplication.ServiceProvider.GetRequiredService<ILoggerFactory>();
        _logger = factory.CreateLogger("Logger");
    }

    public static void LogDebug(string message, params object?[] args)
    {
        _logger.LogDebug(message, args);
    }

    public static void LogError(string message, params object?[] args)
    {
        _logger.LogError(message, args);
    }

    public static void LogInformation(string message, params object?[] args)
    {
        _logger.LogInformation(message, args);
    }

    public static void LogTrace(string message, params object?[] args)
    {
        _logger.LogTrace(message, args);
    }
}