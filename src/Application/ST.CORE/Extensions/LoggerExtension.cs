using Microsoft.AspNetCore.Hosting;
using NLog.Web;
using NLog;
using Microsoft.Extensions.Logging;
using ST.CORE.LoggerTargets;

namespace ST.CORE.Extensions
{
	public static class LoggerExtension
	{
		public static IWebHostBuilder StartLogging(this IWebHostBuilder builder)
		{
			builder.ConfigureServices(services =>
			{
				var config = new NLog.Config.LoggingConfiguration();
				var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
				var customTarget = new LoggerTarget();
				config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, logconsole);
				config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, customTarget);

				LogManager.Configuration = config;
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
