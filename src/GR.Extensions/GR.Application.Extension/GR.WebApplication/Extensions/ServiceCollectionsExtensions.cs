using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor.MsDependencyInjection;
using GR.Cache.Abstractions.Exceptions;
using GR.Cache.Abstractions.Extensions;
using GR.Cache.Services;
using GR.Core;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.ModelBinders.ModelBinderProviders;
using GR.Core.Razor.Extensions;
using GR.Localization.Abstractions.Extensions;
using GR.Localization.Abstractions.Models;
using GR.Notifications.Extensions;
using GR.PageRender.Abstractions.Extensions;
using GR.WebApplication.Helpers;
using GR.WebApplication.Helpers.AppConfigurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace GR.WebApplication.Extensions
{
    public static class ServiceCollectionsExtensions
    {
        /// <summary>
        /// Register gear app
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configAction"></param>
        /// <returns></returns>
        public static IServiceProvider RegisterGearWebApp(this IServiceCollection services, Action<GearServiceCollectionConfig> configAction)
        {
            var configuration = new GearServiceCollectionConfig
            {
                GearServices = services
            };
            configAction(configuration);

            //Register system config
            services.RegisterSystemConfig(configuration.Configuration);

            services.Configure<FormOptions>(x => x.ValueCountLimit =
                configuration.ServerConfiguration.UploadMaximSize);

            //Global settings
            services.AddMvc(options =>
                {
                    options.EnableEndpointRouting = false;
                    options.ModelBinderProviders.Insert(0, new GearDictionaryModelBinderProvider());
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(x =>
                {
                    x.SerializerSettings.DateFormatString = GearSettings.Date.DateFormat;
                });

            services.AddGearSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddGearSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddUrlHelper();

            //Register core razor
            services.RegisterCoreRazorModule();

            //Use compression
            if (configuration.AddResponseCompression && configuration.HostingEnvironment.IsProduction()) services.AddResponseCompression();

            services.AddHttpClient();

            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                // enables immediate logout, after updating the user's stat.
                options.ValidationInterval = TimeSpan.Zero;
            });

            //--------------------------------------Cors origin Module-------------------------------------
            if (configuration.UseDefaultCorsConfiguration)
                services.AddOriginCorsModule();

            //---------------------------------Custom cache Module-------------------------------------
            if (configuration.CacheConfiguration.UseDistributedCache &&
                configuration.CacheConfiguration.UseInMemoryCache)
                throw new InvalidCacheConfigurationException("Both types of cached storage cannot be used");

            if (configuration.CacheConfiguration.UseDistributedCache)
            {
                services.AddDistributedMemoryCache()
                .AddCacheModule<DistributedCacheService, RedisConnection>(configuration.HostingEnvironment, configuration.Configuration);
            }
            else if (configuration.CacheConfiguration.UseInMemoryCache)
            {
                services.AddCacheModule<InMemoryCacheService, RedisConnection>(configuration.HostingEnvironment, configuration.Configuration);
            }

            //---------------------------------Api version Module-------------------------------------
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = configuration.ApiVersioningOptions.ReportApiVersions;
                options.AssumeDefaultVersionWhenUnspecified = configuration.ApiVersioningOptions.AssumeDefaultVersionWhenUnspecified;
                options.DefaultApiVersion = configuration.ApiVersioningOptions.DefaultApiVersion;
                options.ErrorResponses = configuration.ApiVersioningOptions.ErrorResponses;
            });

            //--------------------------------------SignalR Module-------------------------------------
            if (configuration.SignlarConfiguration.UseDefaultConfiguration)
                services.AddSignalRModule();


            //--------------------------------------Swagger Module-------------------------------------
            if (configuration.SwaggerServicesConfiguration.UseDefaultConfiguration)
                services.AddSwaggerModule(configuration.Configuration);

            //Register memory cache
            var cacheService = configuration.BuildGearServices.GetService<IMemoryCache>();
            IoC.Container.Register(Component.For<IMemoryCache>().Instance(cacheService));

            return WindsorRegistrationHelper.CreateServiceProvider(IoC.Container, services);
        }

        /// <summary>
        /// Use gear app
        /// </summary>
        /// <param name="app"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IGearAppBuilder UseGearWebApp(this IApplicationBuilder app, Action<GearAppBuilderConfig> config)
        {
            var configuration = new GearAppBuilderConfig();
            config(configuration);

            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var sp = serviceScope.ServiceProvider;
                var environment = sp.GetService<IHostingEnvironment>();
                GearWebApplication.IsConfigured(environment);

                var lifeTimeService = serviceScope.ServiceProvider.GetService<IApplicationLifetime>();
                lifeTimeService.RegisterAppEvents(app, configuration.AppName);

                //----------------------------------Localization Usage-------------------------------------

                var languages = serviceScope.ServiceProvider.GetService<IOptionsSnapshot<LocalizationConfig>>();
                app.UseLocalizationModule(languages);
            }

            if (GearApplication.IsHostedOnLinux())
            {
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });
            }

            if (configuration.HostingEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            //-----------------------Custom url redirection Usage-------------------------------------
            if (configuration.UseCustomUrlRewrite) app.UseUrlRewriteModule();

            //----------------------------------Origin Cors Usage-------------------------------------
            if (configuration.UseDefaultCorsConfiguration) app.UseConfiguredCors();

            app.UseAuthentication()
                .UseIdentityServer();

            //custom rules
            app.UseAppMvc(configuration.Configuration, configuration.CustomMapRules);

            //--------------------------------------Swagger Usage-------------------------------------
            if (configuration.SwaggerConfiguration.UseSwaggerUI &&
                (configuration.SwaggerConfiguration.UseOnlyInDevelopment && configuration.HostingEnvironment.IsDevelopment() || !configuration.SwaggerConfiguration.UseOnlyInDevelopment))
            {
                app.UseSwagger()
                    .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "GEAR API v1.0");
                });
            }

            //----------------------------------Static files Usage-------------------------------------
            if (configuration.AppFileConfiguration.UseDefaultFiles)
                app.UseDefaultFiles();

            if (configuration.AppFileConfiguration.UseStaticFile)
                app.UseStaticFiles();

            //--------------------------------------Use compression-------------------------------------
            if (configuration.UseResponseCompression && configuration.HostingEnvironment.IsProduction()) app.UseResponseCompression();


            //---------------------------------------SignalR Usage-------------------------------------
            if (configuration.SignlarAppConfiguration.UseDefaultSignlarConfiguration)
            {
                app.UseSignalRModule(configuration.SignlarAppConfiguration.Path);
            }

            return new GearAppBuilder(app.ApplicationServices);
        }

        /// <summary>
        /// Use app as mvc
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configuration"></param>
        /// <param name="routeMapping"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseAppMvc(this IApplicationBuilder app, IConfiguration configuration, Dictionary<string, Action<HttpContext>> routeMapping = null)
        {
            var isConfigured = configuration.GetValue<bool>("IsConfigured");

            var singleTenantTemplate = isConfigured
                ? "{controller=Home}/{action=Index}"
                : "{controller=Installer}/{action=Index}";

            app.UseMvc(routes =>
            {
                routes.ApplicationBuilder.Use(async (context, next) =>
                {
                    if (routeMapping == null || !isConfigured)
                    {
                        await next();
                    }
                    else
                    {
                        var match = routeMapping.FirstOrDefault(o => o.Key.Equals(context.Request.Path));
                        if (!match.IsNull())
                        {
                            match.Value.Invoke(context);
                            await next();
                        }
                        else
                        {
                            await next();
                        }
                    }
                });

                routes.MapRoute(
                    name: "default",
                    template: singleTenantTemplate,
                    defaults: singleTenantTemplate
                //constraints: new { tenant = new TenantRouteConstraint() }
                );
            });
            return app;
        }
    }
}
