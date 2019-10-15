using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ST.TaskManager.Abstractions.BackgroundServices
{
    public class TaskManagerBackgroundService : IHostedService, IDisposable
    {
        /// <summary>
        /// Timer
        /// </summary>
        private Timer _timer;

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger _logger;

        private readonly ITaskManagerNotificationService _managerNotificationService;

        public TaskManagerBackgroundService(ILogger<TaskManagerBackgroundService> logger, ITaskManagerNotificationService managerNotificationService)
        {
            _logger = logger;
            _managerNotificationService = managerNotificationService;
        }

        /// <inheritdoc />
        /// <summary>
        /// Start async
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("TaskManager Background Service is starting.");
            _timer = new Timer(Execute, null, TimeSpan.Zero,
                TimeSpan.FromHours(24));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Send logs
        /// </summary>
        /// <param name="state"></param>
        private async void Execute(object state)
        {
            await _managerNotificationService.TaskExpirationNotificationAsync();
        }

        /// <inheritdoc />
        /// <summary>
        /// Stop async
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Task Manager Background Service is stopping.");

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
