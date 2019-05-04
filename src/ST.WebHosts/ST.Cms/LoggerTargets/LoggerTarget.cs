using NLog;
using NLog.Targets;

namespace ST.Cms.LoggerTargets
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
		protected override  void Write(LogEventInfo logEvent)
		{
			//var message = Layout.Render(logEvent);

			//logEvent.Message = $"{DateTime.Now} {logEvent.FormattedMessage}";
			//await SendToBrowserAsync(new List<LogEventInfo> { logEvent });
			base.Write(logEvent);
		}

		///// <summary>
		///// Send logs
		///// </summary>
		///// <param name="logs"></param>
		///// <returns></returns>
		//private static Task SendToBrowserAsync(IEnumerable<LogEventInfo> logs)
		//{
		//	return Task.CompletedTask;
		//	//try
		//	//{
		//	//	var hubContext = IoC.Resolve<IHubContext<NotificationsHub>>();
		//	//	await hubContext.Clients.All.SendAsync(SignalrSendMethods.SendLog, JsonConvert.SerializeObject(logs.Select(x => new
		//	//	{
		//	//		x.Message,
		//	//		x.Level
		//	//	}).ToList()));
		//	//}
		//	//catch (Exception ex)
		//	//{
		//	//	Console.WriteLine(ex);
		//	//}
		//}
	}
}
