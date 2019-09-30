using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RazorLight;
using ST.Core.Attributes.Documentation;
using ST.Core.Events;
using ST.Core.Extensions;
using ST.Core.Helpers;
using ST.Dashboard.Abstractions.Helpers.Compilers;
using ST.Dashboard.Abstractions.ServiceBuilder;

namespace ST.Dashboard.Abstractions.Extensions
{
    [Author("Lupei Nicolae", 1.1)]
    [Documentation("This extensions is for register dashboard module requirements")]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add dashboard module
        /// </summary>
        /// <typeparam name="TRepository"></typeparam>
        /// <typeparam name="TWidgetGroupRepository"></typeparam>
        /// <typeparam name="TWidgetService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IDashboardServiceCollection AddDashboardModule<TRepository, TWidgetGroupRepository, TWidgetService>(this IServiceCollection services)
            where TRepository : class, IDashboardService
            where TWidgetGroupRepository : class, IWidgetGroupRepository
            where TWidgetService : class, IWidgetService
        {
            IoC.RegisterTransientService<IDashboardService, TRepository>();
            IoC.RegisterTransientService<IWidgetGroupRepository, TWidgetGroupRepository>();
            IoC.RegisterTransientService<IWidgetService, TWidgetService>();
            return new DashboardServiceCollection(services);
        }

        /// <summary>
        /// Add dashboard module storage
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="configuration"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IDashboardServiceCollection AddDashboardModuleStorage<TDbContext>(
            this IDashboardServiceCollection configuration, Action<DbContextOptionsBuilder> options)
            where TDbContext : DbContext, IDashboardDbContext
        {
            Arg.NotNull(configuration.Services, nameof(AddDashboardModuleStorage));
            configuration.Services.AddDbContext<TDbContext>(options, ServiceLifetime.Transient);
            configuration.Services.AddScopedContextFactory<IDashboardDbContext, TDbContext>();
            return configuration;
        }

        /// <summary>
        /// Register services
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IDashboardServiceCollection RegisterDashboardEvents(this IDashboardServiceCollection configuration)
        {
            //on application start seed widgets
            SystemEvents.Application.OnApplicationStarted += async (sender, args) =>
            {
                var service = IoC.Resolve<IDashboardService>();
                if (service != null)
                {
                    await service.SeedWidgetsAsync();
                }
            };
            return configuration;
        }

        /// <summary>
        /// Register render service
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="renders"></param>
        /// <returns></returns>
        public static IDashboardServiceCollection AddDashboardRenderServices(this IDashboardServiceCollection configuration, Dictionary<Type, Type> renders)
        {
            IoC.RegisterServiceCollection(renders);
            return configuration;
        }

        /// <summary>
        /// Register program assembly
        /// </summary>
        /// <param name="conf"></param>
        /// <param name="programType"></param>
        /// <returns></returns>
        public static IDashboardServiceCollection RegisterProgramAssembly(this IDashboardServiceCollection conf, Type programType)
        {
            if (programType.Name.Equals("Program").Negate()) throw new Exception("Incorrect program type");
            var engineBuilder = new RazorLightEngineBuilder()
                .UseEmbeddedResourcesProject(programType)
                .UseMemoryCachingProvider();

            RazorCompilerEngine.RegisterEngine(engineBuilder);
            return conf;
        }
    }
}
