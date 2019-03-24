using Microsoft.Extensions.DependencyInjection;
using ST.DynamicEntityStorage.Abstractions;

namespace ST.DynamicEntityStorage.Extensions
{
    public static class RegisterServiceCollection
    {
        /// <summary>
        /// Register new data access services
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterDynamicDataServices(this IServiceCollection services)
        {
            services.AddTransient<IDynamicService, DynamicService>();
            services.AddTransient<IDynamicDataGetService, DynamicService>();
            services.AddTransient<IDynamicDataCreateService, DynamicService>();
            services.AddTransient<IDynamicDataUpdateService, DynamicService>();
            return services;
        }
    }
}
