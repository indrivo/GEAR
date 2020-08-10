using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GR.Core.Services
{
    public class GearApplicationQueuedHostedService : BackgroundService
    {
        #region Injectable

        /// <summary>
        /// Inject logger
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Inject service provider
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        #endregion


        public GearApplicationQueuedHostedService(ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = loggerFactory.CreateLogger<QueuedHostedService>();
        }


        protected override async Task ExecuteAsync(
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Queued Hosted Service is starting.");

            while (!cancellationToken.IsCancellationRequested)
            {
                var workItem = await GearApplication.BackgroundTaskQueue.RemoveBackgroundWorkItemFromQueueAsync(cancellationToken);

                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        await workItem(scope.ServiceProvider, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        $"Error occurred executing {nameof(workItem)}.");
                }
            }

            _logger.LogInformation("Queued Hosted Service is stopping.");
        }
    }
}
