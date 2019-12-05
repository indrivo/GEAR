using System;
using GR.Application.Middleware.Extensions;
using GR.Application.Middleware.Server;
using GR.Cache.Abstractions.Exceptions;
using GR.Cache.Abstractions.Extensions;
using GR.Cache.Services;
using GR.Core;
using GR.Core.Extensions;
using GR.Core.Helpers.ModelBinders.ModelBinderProviders;
using GR.Core.Razor.Extensions;
using GR.Identity.Abstractions;
using GR.Identity.Data;
using GR.Localization.Abstractions.Extensions;
using GR.Localization.Abstractions.Models;
using GR.Notifications.Extensions;
using GR.WebApplication.Helpers;
using GR.WebApplication.Helpers.AppConfigurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

            //Use compression
            if (configuration.AddResponseCompression && configuration.HostingEnvironment.IsProduction()) services.AddResponseCompression();

            services.AddHttpClient();

            //Register system config
            services.RegisterSystemConfig(configuration.Configuration);

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
                .AddCacheModule<RedisDistributedCacheService, RedisConnection>(configuration.HostingEnvironment, configuration.Configuration);
            }
            else if (configuration.CacheConfiguration.UseInMemoryCache)
            {
                services.AddCacheModule<InMemoryCacheService, RedisConnection>(configuration.HostingEnvironment, configuration.Configuration);
            }

            //Global settings
            services.AddMvc(options =>
                {
                    options.ModelBinderProviders.Insert(0, new GearDictionaryModelBinderProvider());
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(x =>
                {
                    x.SerializerSettings.DateFormatString = GearSettings.Date.DateFormat;
                });

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
                services.AddSignalRModule<ApplicationDbContext, ApplicationUser, ApplicationRole>();


            //--------------------------------------Swagger Module-------------------------------------
            if (configuration.SwaggerServicesConfiguration.UseDefaultConfiguration)
                services.AddSwaggerModule(configuration.Configuration, configuration.HostingEnvironment);

            return services.AddWindsorContainers();
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
                var environment = serviceScope.ServiceProvider.GetService<IHostingEnvironment>();
                GearWebApplication.IsConfigured(environment);

                var lifeTimeService = serviceScope.ServiceProvider.GetService<IApplicationLifetime>();
                lifeTimeService.RegisterAppEvents(app, "GEAR_APP");

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
            if (configuration.UseDefaultCorsConfiguration) app.UseConfiguredCors(configuration.Configuration);

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
    }
}
