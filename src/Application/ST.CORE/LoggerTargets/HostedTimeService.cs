using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ST.Notifications.Abstraction;
using ST.Notifications.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using System.Diagnostics;

namespace ST.CORE.LoggerTargets
{
	public class HostedTimeService : IHostedService, IDisposable
	{
		/// <summary>
		/// Timer
		/// </summary>
		private Timer _timer;
		/// <summary>
		/// Logger
		/// </summary>
		private readonly ILogger _logger;

		/// <summary>
		/// Inject notifier
		/// </summary>
		private readonly INotify _notify;
		/// <summary>
		/// Hub
		/// </summary>
		private readonly IHubContext<NotificationsHub> _hubContext;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="logger"></param>
		/// <param name="notify"></param>
		/// <param name="hubContext"></param>
		public HostedTimeService(ILogger<HostedTimeService> logger, INotify notify, IHubContext<NotificationsHub> hubContext)
		{
			_logger = logger;
			_notify = notify;
			_hubContext = hubContext;
		}

		/// <inheritdoc />
		/// <summary>
		/// Start async
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public Task StartAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("Timed Background Service is starting.");

			_timer = new Timer(SendLogs, null, TimeSpan.Zero,
				TimeSpan.FromSeconds(6));

			return Task.CompletedTask;
		}

		/// <summary>
		/// Send logs
		/// </summary>
		/// <param name="state"></param>
		private async void SendLogs(object state)
		{
			try
			{
				if (!LoggerTarget.Logs.Any()) return;
				await SendToBrowserAsync(LoggerTarget.Logs.ToList());
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		/// <summary>
		/// Send logs
		/// </summary>
		/// <param name="logs"></param>
		/// <returns></returns>
		private async Task SendToBrowserAsync(IEnumerable<LogEventInfo> logs)
		{
			try
			{
				await _hubContext.Clients.All.SendAsync(SignalrSendMethods.SendLog, JsonConvert.SerializeObject(logs.Select(x => new
				{
					x.Message,
					x.Level
				}).ToList()));
				LoggerTarget.Logs.Clear();
				//_logger.LogError("Fail \n error");
				//_logger.LogWarning("Fail \n warn");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		/// <summary>
		/// Stop async
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public Task StopAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("Timed Background Service is stopping.");

			_timer?.Change(Timeout.Infinite, 0);

			return Task.CompletedTask;
		}

		/// <summary>
		/// Dispose timer
		/// </summary>
		public void Dispose()
		{
			_timer?.Dispose();
		}

	}
}
