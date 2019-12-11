using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GR.Backup.Abstractions.Models;

namespace GR.Backup.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register backup runner
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterDatabaseBackupRunnerModule<TRunner, TBackupSettings, TBackupService>(this IServiceCollection services, IConfiguration configuration)
            where TRunner : class, IHostedService
            where TBackupSettings : BackupSettings
            where TBackupService : class, IBackupService<TBackupSettings>
        {
            services.AddTransient<IBackupService<TBackupSettings>, TBackupService>();
            services.Configure<TBackupSettings>(configuration.GetSection(nameof(BackupSettings)));

            //Run background service for backup database
            services.AddHostedService<TRunner>();
            return services;
        }
    }
}