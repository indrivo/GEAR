using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ST.Entities.Data;
using ST.Entities.Extensions;
using ST.Entities.Utils;
using ST.Identity.Data;
using ST.Identity.Extensions;
using ST.Procesess.Data;
using ST.ProcessEngine.Services.HostedServices;

namespace ST.ProcessEngine
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// Hosting configuration
        /// </summary>
        private IHostingEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = ConnectionString.Get(Configuration, HostingEnvironment);
            services.AddDbContext<EntitiesDbContext>(options =>
            {
                options = options.GetDefaultOptions(Configuration, HostingEnvironment);
            });

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options = options.GetDefaultOptions(Configuration, HostingEnvironment);
            });

            services.AddDbContext<ProcessesDbContext>(options =>
            {
                options = options.GetDefaultOptions(Configuration, HostingEnvironment);
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            //Run background service
            services.AddHostedService<ProcessEngineRunner>();
            //Use custom cache service
            services.UseCustomCacheService(HostingEnvironment);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
