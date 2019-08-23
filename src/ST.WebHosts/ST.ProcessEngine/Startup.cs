using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ST.Cache.Extensions;
using ST.Core.Extensions;
using ST.Entities.Data;
using ST.Identity.Data;
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
            services.AddDbContext<EntitiesDbContext>(options =>
            {
                options.GetDefaultOptions(Configuration);
            });

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.GetDefaultOptions(Configuration);
            });

            services.AddDbContext<ProcessesDbContext>(options =>
            {
                options.GetDefaultOptions(Configuration);
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            //Run background service
            services.AddHostedService<ProcessEngineRunner>();
            //Use custom cache service
            services.AddCacheModule(HostingEnvironment, Configuration, "ST.ISO");
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
