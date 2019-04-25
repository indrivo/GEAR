using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ST.Notifications.Abstraction;
using ST.Notifications.Hubs;
using System;
using System.Threading;
using System.Threading.Tasks;
using ST.Identity.Data.Permissions;
using ST.Notifications.Abstractions;
using ILogger = Microsoft.Extensions.Logging.ILogger;

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
		private readonly INotify<ApplicationRole> _notify;
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
		public HostedTimeService(ILogger<HostedTimeService> logger, INotify<ApplicationRole> notify, IHubContext<NotificationsHub> hubContext)
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

			_timer = new Timer(Execute, null, TimeSpan.Zero,
				TimeSpan.FromSeconds(6));

			return Task.CompletedTask;
		}

		/// <summary>
		/// Send logs
		/// </summary>
		/// <param name="state"></param>
		private void Execute(object state)
		{
			//Some actions
		}

		/// <inheritdoc />
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

		/// <inheritdoc />
		/// <summary>
		/// Dispose timer
		/// </summary>
		public void Dispose()
		{
			_timer?.Dispose();
		}
	}
}
