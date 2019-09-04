using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using LogLevel = NLog.LogLevel;

namespace ST.Application.Extensions
{
    public static class LoggerExtension
    {
        public static IWebHostBuilder StartLogging(this IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var config = new NLog.Config.LoggingConfiguration();
                var logConsole = new NLog.Targets.ConsoleTarget();
                //var customTarget = new LoggerTarget();
                config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, logConsole);
                //config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, customTarget);

                LogManager.Configuration = config;

                LogManager.Configuration.AddSentry(o =>
                {
                    // Optionally specify a separate format for message
                    o.Layout = "${message}";
                    // Optionally specify a separate format for breadcrumbs
                    o.BreadcrumbLayout = "${logger}: ${message}";

                    // Debug and higher are stored as breadcrumbs (default is Info)
                    o.MinimumBreadcrumbLevel = LogLevel.Debug;
                    // Error and higher is sent as event (default is Error)
                    o.MinimumEventLevel = LogLevel.Error;

                    // Send the logger name as a tag
                    o.AddTag("logger", "${logger}");

                    // All Sentry Options are accessible here.
                });
            });

            builder.ConfigureLogging(logging =>
            {
                //logging.ClearProviders();
                logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
            })
            .UseNLog();

            return builder;
        }
    }
}
