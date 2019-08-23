using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ST.Core.Extensions;
using ST.Core.Helpers;

namespace ST.TaskManager.Abstractions.Extensions
{

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFileModule<TTaskService>(this IServiceCollection services)
            where TTaskService : class, ITaskManager
        {
            services.AddTransient<ITaskManager, TTaskService>();
            IoC.RegisterService<ITaskManager, TTaskService>();
            return services;
        }


        public static IServiceCollection AddFileModuleStorage<TTaskManagerContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
            where TTaskManagerContext : DbContext, ITaskManagerContext
        {
            services.AddScopedContextFactory<ITaskManagerContext, TTaskManagerContext>();
            services.AddDbContext<TTaskManagerContext>(options, ServiceLifetime.Transient);
            return services;
        }
    }
}
