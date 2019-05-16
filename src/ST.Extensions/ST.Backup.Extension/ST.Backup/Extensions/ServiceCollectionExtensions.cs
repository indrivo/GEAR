using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ST.Backup.BackgroundServices;

namespace ST.Backup.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register backup runner
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterDatabaseBackupRunnerModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<BackupSettings>(configuration.GetSection(nameof(BackupSettings)));

            //Run background service for backup database
            services.AddHostedService<BackupTimeService>();
            return services;
        }
    }
}
