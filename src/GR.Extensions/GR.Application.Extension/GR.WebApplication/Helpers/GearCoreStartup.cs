using System;
using System.Reflection;
using GR.WebApplication.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GR.WebApplication.Helpers
{
    public abstract class GearCoreStartup
    {
        #region Injectable

        /// <summary>
        /// AppSettings configuration
        /// </summary>
        protected virtual IConfiguration Configuration { get; }

        /// <summary>
        /// Hosting configuration
        /// </summary>
        protected virtual IHostingEnvironment HostingEnvironment { get; }

        #endregion

        /// <summary>
        /// Migrations Assembly
        /// </summary>
        protected static readonly string MigrationsAssembly =
            typeof(Identity.DbSchemaNameConstants).GetTypeInfo().Assembly.GetName().Name;

        protected GearCoreStartup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// Configure cms app
        /// </summary>
        /// <param name="app"></param>
        public virtual void Configure(IApplicationBuilder app)
            => app.UseGearWebApp(config =>
            {
                config.HostingEnvironment = HostingEnvironment;
                config.Configuration = Configuration;
            });

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public abstract IServiceProvider ConfigureServices(IServiceCollection services);
    }
}
