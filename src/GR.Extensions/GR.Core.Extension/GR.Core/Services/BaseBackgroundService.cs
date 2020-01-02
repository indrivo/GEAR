using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace GR.Core.Services
{
    public abstract class BaseBackgroundService<TBackgroundService> : IHostedService, IDisposable
        where TBackgroundService : class, IHostedService
    {
        /// <summary>
        /// Timer
        /// </summary>
        protected Timer Timer;

        /// <summary>
        /// Logger
        /// </summary>
        protected readonly ILogger<TBackgroundService> Logger;

        /// <summary>
        /// Service name
        /// </summary>
        protected readonly string ServiceName;

        /// <summary>
        /// Interval
        /// </summary>
        protected TimeSpan Interval { get; set; } = TimeSpan.FromHours(24);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="logger"></param>
        protected BaseBackgroundService(string serviceName, ILogger<TBackgroundService> logger)
        {
            Logger = logger;
            ServiceName = serviceName;
        }

        /// <inheritdoc />
        /// <summary>
        /// Start async
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation($"{ServiceName} Background Service is starting.");
            Timer = new Timer(async o => await Execute(o), null, TimeSpan.Zero,
                Interval);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Send logs
        /// </summary>
        /// <param name="state"></param>
        public abstract Task Execute(object state);

        /// <inheritdoc />
        /// <summary>
        /// Stop async
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation($"{ServiceName} Background Service is stopping.");

            Timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        /// <summary>
        /// Dispose timer
        /// </summary>
        public void Dispose()
        {
            Timer?.Dispose();
        }
    }
}
