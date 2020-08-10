using System;
using GR.Audit.Abstractions.Extensions;
using GR.Core;
using GR.Core.Events;
using GR.Core.Extensions;
using GR.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GR.WebHooks.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add web hook module  
        /// </summary>
        /// <typeparam name="TIncomingService"></typeparam>
        /// <typeparam name="TOutgoingService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddWebHookModule<TIncomingService, TOutgoingService>(this IServiceCollection services)
            where TIncomingService : class, IIncomingHookService
            where TOutgoingService : class, IOutgoingHookService
        {
            services.AddGearSingleton<IIncomingHookService, TIncomingService>();
            services.AddGearSingleton<IOutgoingHookService, TOutgoingService>();
            SystemEvents.Application.OnEvent += (sender, args) =>
            {
                if (!GearApplication.Configured) return;
                GearApplication.BackgroundTaskQueue.PushBackgroundWorkItemInQueue(async (s, p) =>
                {
                    var service = s.GetService<IOutgoingHookService>();
                    await service.SendEventAsync(args);
                });
            };
            return services;
        }

        /// <summary>
        /// Add web hook storage
        /// </summary>
        /// <typeparam name="TFormContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddWebHookModuleStorage<TFormContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
            where TFormContext : DbContext, IWebHookContext
        {
            Arg.NotNull(options, nameof(AddWebHookModuleStorage));
            services.AddDbContext<TFormContext>(options);
            services.RegisterAuditFor<IWebHookContext>("WebHook module");
            SystemEvents.Database.OnAllMigrate += (sender, args) =>
            {
                GearApplication.GetHost().MigrateDbContext<TFormContext>();
            };
            return services;
        }
    }
}