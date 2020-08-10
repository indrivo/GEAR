﻿using GR.Core;
using Microsoft.Extensions.DependencyInjection;
using GR.Forms.Razor.Helpers;
using GR.UI.Menu.Abstractions;
using GR.UI.Menu.Abstractions.Events;

namespace GR.Forms.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register page render
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddFormStaticFilesModule(this IServiceCollection services)
        {
            services.ConfigureOptions(typeof(FormFileConfiguration));
            MenuEvents.Menu.OnMenuSeed += (sender, args) =>
            {
                GearApplication.BackgroundTaskQueue.PushBackgroundWorkItemInQueue(async (s, p) =>
                    {
                        await s.GetService<IMenuService>().AppendMenuItemsAsync(new FormsMenuInitializer());
                    });
            };
            return services;
        }
    }
}
