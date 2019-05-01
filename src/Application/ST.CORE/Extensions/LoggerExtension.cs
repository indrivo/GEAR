using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;

namespace ST.WebHost.Extensions
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
