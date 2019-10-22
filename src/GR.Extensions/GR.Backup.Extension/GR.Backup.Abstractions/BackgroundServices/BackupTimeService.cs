using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using GR.Backup.Abstractions.Exceptions;
using GR.Backup.Abstractions.Models;

namespace GR.Backup.Abstractions.BackgroundServices
{
    public class BackupTimeService<T> : IHostedService, IDisposable where T : BackupSettings, new()
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
        /// Settings
        /// </summary>
        private readonly T _settings;

        /// <summary>
        /// Inject backup service
        /// </summary>
        private readonly IBackupService<T> _backupService;

        public BackupTimeService(ILogger<BackupTimeService<T>> logger, IOptions<T> options, IBackupService<T> backupService)
        {
            _logger = logger;
            _backupService = backupService;
            _settings = options.Value ?? throw new BackupSettingsNotRegisteredException();
        }

        /// <inheritdoc />
        /// <summary>
        /// Start async
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Backup Background Service is starting.");
            if (!_settings.Enabled) await StopAsync(cancellationToken);
            _timer = new Timer(Execute, null, TimeSpan.Zero,
            TimeSpan.FromHours(_settings.Interval));
        }

        /// <summary>
        /// Send logs
        /// </summary>
        /// <param name="state"></param>
        private void Execute(object state)
        {
            if (!_settings.Enabled) return;
            _logger.LogInformation($"Backup Background Service run for {_backupService.GetProviderName()}");
            _backupService.Backup();
        }

        /// <inheritdoc />
        /// <summary>
        /// Stop async
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Backup Background Service is stopping.");

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
