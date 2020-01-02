using System;
using System.Linq;
using GR.Audit.Abstractions.Extensions;
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
        /// <typeparam name="TWorkFlowCreator"></typeparam>
        /// <typeparam name="TWorkFlowExecutor"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddWorkFlowModule<TEntity, TWorkFlowCreator, TWorkFlowExecutor>(this IServiceCollection services)
            where TWorkFlowCreator : class, IWorkFlowCreatorService<TEntity>
            where TWorkFlowExecutor : class, IWorkFlowExecutorService
            where TEntity : WorkFlow
        {
            services.AddGearTransient<IWorkFlowCreatorService<TEntity>, TWorkFlowCreator>();

            services.AddGearTransient<IWorkFlowExecutorService, TWorkFlowExecutor>();
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
            services.RegisterAuditFor<IWorkFlowContext>($"{nameof(WorkFlow)} module");
            SystemEvents.Database.OnMigrate += (sender, args) =>
            {
                GearApplication.GetHost<IWebHost>().MigrateDbContext<TContext>();
            };
            return services;
        }

        /// <summary>
        /// Register entity contract
        /// </summary>
        /// <param name="services"></param>
        /// <param name="entityName"></param>
        /// <param name="workflowId"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterWorkFlowContract(this IServiceCollection services, string entityName, Guid? workflowId)
        {
            SystemEvents.Application.OnApplicationStarted += async (sender, args) =>
            {
                if (!GearApplication.Configured) return;
                var service = IoC.Resolve<IWorkFlowExecutorService>();
                await service.RegisterEntityContractToWorkFlowAsync(entityName, workflowId);
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
            SystemEvents.Application.OnApplicationStarted += async (sender, args) =>
            {
                if (!GearApplication.Configured) return;
                var type = typeof(TAction);
                var name = type.Name;
                var fullName = type.FullName;
                var service = IoC.Resolve<IWorkFlowContext>();
                if (!service.WorkflowActions.Any(x => x.Name.Equals(name)))
                {
                    await service.WorkflowActions.AddAsync(new WorkflowAction
                    {
                        Name = name,
                        ClassName = name,
                        ClassNameWithNameSpace = fullName,
                    });
                    await service.PushAsync();
                }
            };
            return services;
        }
    }
}
