//using Serilog;
//using Serilog.Sinks.Loki;
//using Serilog.Sinks.Loki.Labels;


//namespace MetricsTest;

//internal class Program
//{
//    private static void Main(string[] args)
//    {

//        // var credentials = new BasicAuthCredentials("http://localhost:3100", "<username>", "<password>");
//        var credentials = new NoAuthCredentials("http://localhost:3100"); // Address to local or remote Loki server


//        var defaultLabelProvider = new DefaultLogLabelProvider(new[] { new LokiLabel("name", "tabbycat") });


//        Log.Logger = new LoggerConfiguration()
//            .MinimumLevel.Information()
//            .Enrich.FromLogContext()
//            .WriteTo.LokiHttp(()=>new LokiSinkConfiguration(){ LokiUrl = "http://24.233.2.12:3100",LogLabelProvider =defaultLabelProvider})
//            .CreateLogger();


//        Log.Error("test");

//        var position = new { Latitude = 25, Longitude = 134 };
//        var elapsedMs = 34;
//        Log.Information("Message processed {@Position} in {Elapsed:000} ms.", position, elapsedMs);

//        Log.CloseAndFlush();
//    }

//}