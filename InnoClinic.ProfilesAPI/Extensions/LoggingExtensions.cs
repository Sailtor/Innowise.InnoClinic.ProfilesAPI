using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace InnoClinic.ProfilesAPI.Extensions
{
    public static class LoggingExtensions
    {
        public static void CongigureLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(new JsonFormatter(), "logs/log.txt",
                    rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true,
                    fileSizeLimitBytes: 524288000,
                    retainedFileCountLimit: 31)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .CreateLogger();
        }

    }
}
