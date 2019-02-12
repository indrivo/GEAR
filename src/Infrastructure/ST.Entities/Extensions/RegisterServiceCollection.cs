using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ST.BaseBusinessRepository;
using ST.Entities.Data;
using ST.Entities.Services;
using ST.Entities.Services.Abstraction;

namespace ST.Entities.Extensions
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
            services.AddTransient<IDynamicEntityDataService, DynamicEntityDataService>();
            services.AddTransient<IDynamicEntityGetService, DynamicEntityDataService>();
            services.AddTransient<IDynamicEntityCreateService, DynamicEntityDataService>();
            services.AddTransient<IDynamicEntityUpdateService, DynamicEntityDataService>();
            return services;
        }
    }
}
