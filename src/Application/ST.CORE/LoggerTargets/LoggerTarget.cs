using NLog;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using ST.CORE.Extensions;
using ST.Notifications.Hubs;

namespace ST.CORE.LoggerTargets
{
	[Target("CustomLogger")]
	public sealed class LoggerTarget : TargetWithLayout
	{
		/// <inheritdoc />
		/// <summary>
		/// Constructor
		/// </summary>
		public LoggerTarget()
		{

		}

		/// <inheritdoc />
		/// <summary>
		/// Write logs
		/// </summary>
		/// <param name="logEvent"></param>
		protected override async void Write(LogEventInfo logEvent)
		{
			//var message = Layout.Render(logEvent);

			logEvent.Message = $"{DateTime.Now} {logEvent.FormattedMessage}";
			await SendToBrowserAsync(new List<LogEventInfo> { logEvent });
		}

		/// <summary>
		/// Send logs
		/// </summary>
		/// <param name="logs"></param>
		/// <returns></returns>
		private static async Task SendToBrowserAsync(IEnumerable<LogEventInfo> logs)
		{
			//try
			//{
			//	var hubContext = IoC.Resolve<IHubContext<NotificationsHub>>();
			//	await hubContext.Clients.All.SendAsync(SignalrSendMethods.SendLog, JsonConvert.SerializeObject(logs.Select(x => new
			//	{
			//		x.Message,
			//		x.Level
			//	}).ToList()));
			//}
			//catch (Exception ex)
			//{
			//	Console.WriteLine(ex);
			//}
		}
	}
}
