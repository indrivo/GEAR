using System;
using GR.Core;
using GR.Core.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.TaskManager.Abstractions.BackgroundServices;
using Microsoft.AspNetCore.Hosting;

namespace GR.TaskManager.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register task module
        /// </summary>
        /// <typeparam name="TTaskService"></typeparam>
        /// <typeparam name="TTaskManagerNotificationService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddTaskModule<TTaskService, TTaskManagerNotificationService>(this IServiceCollection services)
            where TTaskService : class, ITaskManager where TTaskManagerNotificationService : class, ITaskManagerNotificationService
        {
            services.AddTransient<ITaskManager, TTaskService>();
            IoC.RegisterTransientService<ITaskManager, TTaskService>();
            services.AddTransient<ITaskManagerNotificationService, TTaskManagerNotificationService>();
            services.RegisterBackgroundService<TaskManagerBackgroundService>();
            return services;
        }

        /// <summary>
        /// Register storage
        /// </summary>
        /// <typeparam name="TTaskManagerContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddTaskModuleStorage<TTaskManagerContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
            where TTaskManagerContext : DbContext, ITaskManagerContext
        {
            services.AddScopedContextFactory<ITaskManagerContext, TTaskManagerContext>();
            services.AddDbContext<TTaskManagerContext>(options, ServiceLifetime.Transient);
            SystemEvents.Database.OnMigrate += (sender, args) =>
            {
                GearApplication.GetHost<IWebHost>().MigrateDbContext<TTaskManagerContext>();
            };
            return services;
        }
    }
}
