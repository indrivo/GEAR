using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GR.Backup.Abstractions.Models;
using GR.Core.Extensions;

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
        public static IServiceCollection RegisterDatabaseBackupRunnerModule<TBackupSettings, TBackupService>(this IServiceCollection services, IConfiguration configuration)
            where TBackupSettings : BackupSettings
            where TBackupService : class, IBackupService
        {
            services.AddGearSingleton<IBackupService, TBackupService>();

            services.Configure<TBackupSettings>(configuration.GetSection(nameof(BackupSettings)));
            return services;
        }

        /// <summary>
        /// Register auto backup runner
        /// </summary>
        /// <typeparam name="TRunner"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterDatabaseBackgroundService<TRunner>(this IServiceCollection services)
            where TRunner : class, IHostedService
        {
            //Register background service for backup database
            services.AddHostedService<TRunner>();
            return services;
        }
    }
}