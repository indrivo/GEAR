using NLog;
using NLog.Targets;
using System;
using System.Collections.Generic;

namespace ST.CORE.LoggerTargets
{
	[Target("CustomLogger")]
	public sealed class LoggerTarget : TargetWithLayout
	{
		/// <summary>
		/// Logs
		/// </summary>
		public static List<LogEventInfo> Logs = new List<LogEventInfo>();

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="host"></param>
		public LoggerTarget()
		{

		}

		/// <summary>
		/// Write logs
		/// </summary>
		/// <param name="logEvent"></param>
		protected override void Write(LogEventInfo logEvent)
		{
			//var message = Layout.Render(logEvent);

			//logEvent.Message = $"{DateTime.Now} {logEvent.FormattedMessage}";
			//Logs.Add(logEvent);
		}
	}
}
