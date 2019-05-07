using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ST.Backup.Exceptions;

namespace ST.Backup.BackgroundServices
{
    public class BackupTimeService : IHostedService, IDisposable
    {
        /// <summary>
        /// Timer
        /// </summary>
        private Timer _timer;

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger _logger;

        private readonly BackupSettings _settings;

        public BackupTimeService(ILogger<BackupTimeService> logger, IOptions<BackupSettings> options)
        {
            _logger = logger;
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
            var userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var directoryPath = Path.Combine(userProfilePath, $"backup\\{_settings.BackupFolder}");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            if (_settings.UsePostGreSql)
            {
                _logger.LogInformation($"Backup Background Service run for {nameof(BackupProvider.PostGreSql)}");
                BackupProvider.PostGreSql.Backup(_settings.PostGreSqlBackupSettings, directoryPath);
            }
            else if (_settings.UseMsSql)
            {
                _logger.LogInformation($"Backup Background Service run for {nameof(BackupProvider.MsSql)}");
                BackupProvider.MsSql.Backup(_settings.MsSqlBackupSettings, directoryPath);
            }
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
