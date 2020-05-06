using Castle.Windsor.MsDependencyInjection;
using GR.Cache.Abstractions.Extensions;
using GR.Cache.Exceptions;
using GR.Cache.Extensions;
using GR.Cache.Helpers;
using GR.Cache.Services;
using GR.Core;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.ModelBinders.ModelBinderProviders;
using GR.Localization.Abstractions.Extensions;
using GR.Localization.Abstractions.Models.Config;
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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AutoMapper;
using GR.Core.Attributes.Validation;
using GR.Core.Helpers.ConnectionStrings;
using GR.Core.Razor.Extensions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Component = Castle.MicroKernel.Registration.Component;

namespace GR.WebApplication.Extensions
{
    public static class ServiceCollectionsExtensions
    {
        /// <summary>
        /// Register gear app
        /// </summary>
        /// <param name="services"></param>
        /// <param name="hostingEnvironment"></param>
        /// <param name="configAction"></param>
        /// <param name="conf"></param>
        /// <returns></returns>
        public static IServiceProvider RegisterGearWebApp(this IServiceCollection services, IConfiguration conf, IHostingEnvironment hostingEnvironment, Action<GearServiceCollectionConfig> configAction)
        {
            IoC.Container.Register(Component.For<IConfiguration>().Instance(conf));
            IoC.Container.Register(Component.For<IHostingEnvironment>().Instance(hostingEnvironment));

            //----------------------------------------Health Check-------------------------------------
            services.AddHealthChecks();
            services.AddDatabaseHealth();
            //services.AddSeqHealth();
            services.AddHealthChecksUI("health-gear-database", setup =>
            {
                setup.SetEvaluationTimeInSeconds((int)TimeSpan.FromMinutes(2).TotalSeconds);
                setup.AddHealthCheckEndpoint(GearApplication.SystemConfig.MachineIdentifier, $"{GearApplication.SystemConfig.EntryUri}hc");
            });



            var configuration = new GearServiceCollectionConfig
            {
                GearServices = services,
                HostingEnvironment = hostingEnvironment,
                Configuration = conf
            };

            configAction(configuration);

            //Register mappings from modules
            services.AddAutoMapper(configuration.GetAutoMapperProfilesFromAllAssemblies().ToArray());

            //Register system config
            services.RegisterSystemConfig(configuration.Configuration);

            services.Configure<FormOptions>(x => x.ValueCountLimit =
                configuration.ServerConfiguration.UploadMaximSize);

            //Global settings
            services.AddMvc(options =>
                {
                    //Global
                    options.EnableEndpointRouting = false;

                    //Binders
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
                .AddCacheModule<DistributedCacheService>()
                .AddRedisCacheConfiguration<RedisConnection>(configuration.HostingEnvironment, configuration.Configuration);
            }
            else if (configuration.CacheConfiguration.UseInMemoryCache)
            {
                services.AddCacheModule<InMemoryCacheService>();
            }

            //---------------------------------Api version Module-------------------------------------
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = configuration.ApiVersioningOptions.ReportApiVersions;
                options.AssumeDefaultVersionWhenUnspecified = configuration.ApiVersioningOptions.AssumeDefaultVersionWhenUnspecified;
                options.DefaultApiVersion = configuration.ApiVersioningOptions.DefaultApiVersion;
                options.ErrorResponses = configuration.ApiVersioningOptions.ErrorResponses;
            });

            //--------------------------------------Swagger Module-------------------------------------
            if (configuration.SwaggerServicesConfiguration.UseDefaultConfiguration)
                services.AddSwaggerModule(configuration.Configuration);

            //Register memory cache
            IoC.Container.Register(Component
                .For<IMemoryCache>()
                .Instance(configuration.BuildGearServices.GetService<IMemoryCache>()));


            //Type Convertors
            TypeDescriptor.AddAttributes(typeof(DateTime), new TypeConverterAttribute(typeof(EuDateTimeConvertor)));

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
            //---------------------------------------Heath Check---------------------------------------
            app.UseHealthChecks("/hc", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = GearHealthCheckWriter.WriteHealthCheckUiResponse
            });

            app.UseHealthChecksUI();

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
                var forwardOptions = new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                };

                if (configuration.HostingEnvironment.IsDevelopment())
                {
                    forwardOptions.ForwardLimit = 2;
                }

                app.UseForwardedHeaders(forwardOptions);
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
                app.UseHsts();
            }

            //----------------------------------Origin Cors Usage-------------------------------------
            if (configuration.UseDefaultCorsConfiguration) app.UseConfiguredCors();

            app.UseCookiePolicy();
            app.UseAuthentication();

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
                );
            });
            return app;
        }

        /// <summary>
        /// Add database health
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddDatabaseHealth(this IServiceCollection services)
        {
            var (dbType, connection) = DbUtil.GetConnectionString(IoC.Resolve<IConfiguration>());
            var builder = services.AddHealthChecks();
            switch (dbType)
            {
                case DbProviderType.MsSqlServer:
                    builder.AddSqlServer(connection, name: "db-MSSQL", tags: new[] { "db", "sql", "sqlserver" });
                    break;
                case DbProviderType.PostgreSql:
                    builder.AddNpgSql(connection, name: "db-POSTGRESQL", tags: new[] { "db", "sql", "postgres" });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return services;
        }

        /// <summary>
        /// Add seq health check
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSeqHealth(this IServiceCollection services)
        {
            var builder = services.AddHealthChecks();
            var configurator = IoC.Resolve<IConfiguration>();
            builder.AddSeqPublisher(setup =>
            {
                setup.ApiKey = configurator.GetValue<string>("Logging:Seq:ApiKey");
                setup.Endpoint = configurator.GetValue<string>("Logging:Seq:ServerUrl");
            }, "SEQ-logger");
            return services;
        }
    }
}