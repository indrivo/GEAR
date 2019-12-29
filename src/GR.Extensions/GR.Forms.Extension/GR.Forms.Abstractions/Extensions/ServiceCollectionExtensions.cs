using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GR.Audit.Abstractions.Extensions;
using GR.Core;
using GR.Core.Events;
using GR.Core.Extensions;
using GR.Core.Helpers;
using Microsoft.AspNetCore.Hosting;

namespace GR.Forms.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add form module
        /// </summary>
        /// <typeparam name="TFormContext"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddFormModule<TFormContext>(this IServiceCollection services) where TFormContext : DbContext, IFormContext
        {
            services.AddScopedContextFactory<IFormContext, TFormContext>();
            return services;
        }

        /// <summary>
        /// Register form service
        /// </summary>
        /// <typeparam name="TFormService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterFormService<TFormService>(this IServiceCollection services)
            where TFormService : class, IFormService
        {
            services.AddTransient<IFormService, TFormService>();
            IoC.RegisterTransientService<IFormService, TFormService>();
            return services;
        }

        /// <summary>
        /// Register form module context
        /// </summary>
        /// <typeparam name="TFormContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddFormModuleStorage<TFormContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
            where TFormContext : DbContext, IFormContext
        {
            Arg.NotNull(options, nameof(AddFormModuleStorage));
            services.AddDbContext<TFormContext>(options);
            services.RegisterAuditFor<IFormContext>("Form module");
            SystemEvents.Database.OnMigrate += (sender, args) =>
            {
                GearApplication.GetHost<IWebHost>().MigrateDbContext<TFormContext>();
            };
            return services;
        }
    }
}
