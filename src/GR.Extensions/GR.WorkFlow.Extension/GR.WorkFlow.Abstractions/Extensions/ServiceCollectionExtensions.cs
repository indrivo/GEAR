using System;
using System.Linq;
using GR.Core;
using GR.Core.Events;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.WorkFlows.Abstractions.Helpers.ActionHandlers;
using GR.WorkFlows.Abstractions.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GR.WorkFlows.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add workflow module
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddWorkFlowModule<TEntity, TService>(this IServiceCollection services)
            where TService : class, IWorkFlowCreatorService<TEntity>
            where TEntity : WorkFlow
        {
            services.AddTransient<IWorkFlowCreatorService<TEntity>, TService>();
            IoC.RegisterTransientService<IWorkFlowCreatorService<TEntity>, TService>();

            services.RegisterWorkflowAction<SendNotificationAction>();

            return services;
        }

        /// <summary>
        /// Add workflow context
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddWorkflowModuleStorage<TContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
            where TContext : DbContext, IWorkFlowContext
        {
            services.AddScopedContextFactory<IWorkFlowContext, TContext>();
            services.AddDbContext<TContext>(options, ServiceLifetime.Transient);
            SystemEvents.Database.OnMigrate += (sender, args) =>
            {
                GearApplication.GetHost<IWebHost>().MigrateDbContext<TContext>();
            };
            return services;
        }

        /// <summary>
        /// Register Workflow action
        /// </summary>
        /// <typeparam name="TAction"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterWorkflowAction<TAction>(this IServiceCollection services)
            where TAction : BaseWorkFlowAction
        {
            var type = typeof(TAction);
            var name = type.Name;
            var fullName = type.FullName;
            var service = IoC.Resolve<IWorkFlowContext>();
            if (!service.WorkflowActions.Any(x => x.Name.Equals(name)))
            {
                service.WorkflowActions.Add(new WorkflowAction
                {
                    Name = name,
                    ClassName = name,
                    ClassNameWithNameSpace = fullName,
                });
            }
            return services;
        }
    }
}
