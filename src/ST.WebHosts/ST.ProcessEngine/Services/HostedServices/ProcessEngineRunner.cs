using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ST.ProcessEngine.Services.HostedServices
{
    public class ProcessEngineRunner : IHostedService, IDisposable
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
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="notify"></param>
        /// <param name="hubContext"></param>
        public ProcessEngineRunner(ILogger<ProcessEngineRunner> logger)
        {
            _logger = logger;
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

            _timer = new Timer(Action, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(6));

            return Task.CompletedTask;
        }

        /// <summary>
        /// Send logs
        /// </summary>
        /// <param name="state"></param>
        private void Action(object state)
        {

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
