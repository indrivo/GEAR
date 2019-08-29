﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ST.Core.Extensions;
using ST.Core.Helpers;

namespace ST.TaskManager.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register task module
        /// </summary>
        /// <typeparam name="TTaskService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddTaskModule<TTaskService>(this IServiceCollection services)
            where TTaskService : class, ITaskManager
        {
            services.AddTransient<ITaskManager, TTaskService>();
            IoC.RegisterService<ITaskManager, TTaskService>();
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
            return services;
        }
    }
}
