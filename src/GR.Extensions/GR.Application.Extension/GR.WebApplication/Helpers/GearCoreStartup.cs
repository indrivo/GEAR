using System;
using System.Reflection;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Extensions;
using GR.UI.Menu.Abstractions;
using GR.WebApplication.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GR.WebApplication.Helpers
{
    /// <summary>
    /// Base Startup.cs configuration for gear .net core web app
    /// </summary>
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
        protected virtual IWebHostEnvironment HostingEnvironment { get; }

        #endregion

        protected GearCoreStartup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
            var appVersion = Assembly.GetAssembly(GetType())?.GetName().Version?.ToString() ?? "1.0.1";
            GearApplication.SetAppVersion(appVersion);
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
        public abstract void ConfigureServices(IServiceCollection services);

        /// <summary>
        /// This method is called when the migrations start to be applied
        /// </summary>
        /// <param name="webHost"></param>
        /// <returns></returns>
        public virtual Task OnBeforeDatabaseMigrationsApply(IHost webHost)
        {
            webHost.MigrateAbstractDbContext<IMenuDbContext>();
            return Task.CompletedTask;
        }
    }
}