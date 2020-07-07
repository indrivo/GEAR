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
using GR.WebApplication.Helpers.AppConfigurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Linq;
using AutoMapper;
using GR.Core.Abstractions;
using GR.Core.Attributes.Validation;
using GR.Core.Helpers.ConnectionStrings;
using GR.Core.Razor.Extensions;
using GR.Core.Services;
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
            IoC.RegisterSingletonService<IAppSender, AppSender>();

            var configuration = new GearServiceCollectionConfig
            {
                GearServices = services,
                HostingEnvironment = hostingEnvironment,
                Configuration = conf
            };

            configAction(configuration);

            //----------------------------------------Health Check-------------------------------------
            if (configuration.UseHealthCheck)
            {
                services.AddHealthChecks();
                services.AddDatabaseHealth();
                //services.AddSeqHealth();
                services.AddHealthChecksUI("health-gear-database", setup =>
                {
                    setup.SetEvaluationTimeInSeconds((int)TimeSpan.FromMinutes(2).TotalSeconds);
                    setup.AddHealthCheckEndpoint(GearApplication.SystemConfig.MachineIdentifier, $"{GearApplication.SystemConfig.EntryUri}hc");
                });
            }

            //Register mappings from modules
            services.AddAutoMapper(configuration.GetAutoMapperProfilesFromAllAssemblies().ToArray());

            //Register system config
            services.RegisterSystemConfig(configuration.Configuration);

            services.Configure<FormOptions>(x => x.ValueCountLimit =
                configuration.ServerConfiguration.UploadMaximSize);

            services.AddResponseCaching();

            //Global settings
            var mvcBuilder = services.AddMvc(options =>
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

            if (configuration.UseHotReload) mvcBuilder.AddGearViewsHotReload();

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
                    .AddRedisCacheConfiguration(configuration.HostingEnvironment, configuration.Configuration);

                services.AddCacheModule<DistributedCacheService>()
                    .AddRedisCacheConnection<RedisConnection>();
            }
            else if (configuration.CacheConfiguration.UseInMemoryCache)
            {
                services.AddCacheModule<InMemoryCacheService>();
            }

            //---------------------------------Api version Module-------------------------------------
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = configuration.ApiVersionOptions.ReportApiVersions;
                options.AssumeDefaultVersionWhenUnspecified = configuration.ApiVersionOptions.AssumeDefaultVersionWhenUnspecified;
                options.DefaultApiVersion = configuration.ApiVersionOptions.DefaultApiVersion;
                options.ErrorResponses = configuration.ApiVersionOptions.ErrorResponses;
            });

            //--------------------------------------Swagger Module-------------------------------------
            if (configuration.SwaggerServicesConfiguration.UseSwagger && configuration.SwaggerServicesConfiguration.UseDefaultConfiguration)
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