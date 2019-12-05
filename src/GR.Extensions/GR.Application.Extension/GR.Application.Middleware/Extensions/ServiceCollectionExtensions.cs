using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using GR.Core.Helpers;
using GR.DynamicEntityStorage;
using GR.DynamicEntityStorage.Abstractions;
using GR.Entities.Data;
using GR.Forms;
using GR.Forms.Abstractions;
using GR.Forms.Data;
using GR.Identity.Abstractions;
using GR.Identity.Data;
using GR.Identity.Data.Groups;
using GR.Identity.Filters;
using GR.Identity.Versioning;
using ST.MPass.Gov;
using GR.Notifications.Abstractions;
using GR.Notifications.Services;
using Swashbuckle.AspNetCore.Swagger;

namespace GR.Application.Middleware.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add services relative to this application
        /// </summary>
        /// <param name="services"></param>
        /// <param name="env"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddApplicationSpecificServices(this IServiceCollection services, IHostingEnvironment env, IConfiguration configuration)
        {
            services.Configure<FormOptions>(x => x.ValueCountLimit = int.MaxValue);
            services.AddTransient<IMPassService, MPassService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IGroupRepository<ApplicationDbContext, ApplicationUser>, GroupRepository<ApplicationDbContext>>();
            services.AddTransient<IFormService, FormService<FormDbContext>>();
            return services;
        }

        /// <summary>
        /// Add authentication
        /// </summary>
        /// <param name="services"></param>
        /// <param name="env"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuthenticationAndAuthorization(this IServiceCollection services,
            IHostingEnvironment env, IConfiguration configuration)
        {
            var authority = configuration.GetSection("WebClients").GetSection("CORE");
            var uri = authority.GetValue<string>("uri");

            services.AddAuthentication()
                .AddJwtBearer(opts =>
                {
                    opts.Audience = "core";
                    opts.Authority = uri;
                    opts.RequireHttpsMetadata = false;
                });
            return services;
        }

        /// <summary>
        /// Add swagger
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public static IServiceCollection AddSwaggerModule(this IServiceCollection services, IConfiguration configuration,
            IHostingEnvironment env)
        {
            // prevent from mapping "sub" claim to name identifier.
            //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            //services.AddConsulServiceDiscovery(Configuration);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            var confAuth = configuration.GetSection("WebClients").GetSection("CORE");
            var authUrl = confAuth.GetValue<string>("uri");
            services.AddSwaggerGen(options =>
            {
                options.DocInclusionPredicate(SwaggerVersioning.DocInclusionPredicate);

                options.SwaggerDoc("v1.0", new Info
                {
                    Title = "GEAR APP HTTP API",
                    Version = "v1.0",
                    Description = "GEAR Service HTTP API",
                    TermsOfService = "Terms Of Service",
                    Contact = new Contact
                    {
                        Url = "http://indrivo.com",
                        Email = "support@indrivo.com",
                        Name = "Indrivo SRL"
                    }
                });

                // Set this flag to omit descriptions for any actions decorated with the Obsolete attribute
                //
                options.IgnoreObsoleteActions();

                // Set this flag to omit schema property descriptions for any type properties decorated with the
                // Obsolete attribute
                //
                options.IgnoreObsoleteProperties();

                // In accordance with the built in JsonSerializer, Swashbuckle will, by default, describe enums as integers.
                // You can change the serializer behavior by configuring the StringToEnumConverter globally or for a given
                // enum type. Swashbuckle will honor this change out-of-the-box. However, if you use a different
                // approach to serialize enums as strings, you can also force Swashbuckle to describe them as strings.
                //
                options.DescribeAllEnumsAsStrings();
                options.DescribeStringEnumsInCamelCase();
                options.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Type = "oauth2",
                    Flow = "implicit",
                    AuthorizationUrl = $"{authUrl}/connect/authorize",
                    TokenUrl = $"{authUrl}/connect/token",
                    Scopes = new Dictionary<string, string>
                    {
                        {"core", "CORE API"}
                    }
                });
                options.OperationFilter<SwaggerAuthorizeCheckOperationFilter>();
                // Integrate XML comments
                if (File.Exists(XmlCommentsFilePath))
                    options.IncludeXmlComments(XmlCommentsFilePath);
            });
            return services;
        }

        /// <summary>
        /// Register windsor containers
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceProvider AddWindsorContainers(this IServiceCollection services)
        {
            var context = services.BuildServiceProvider().GetService<EntitiesDbContext>();
            //var env = services.BuildServiceProvider().GetService<IHostingEnvironment>();

            //Register notifier 
            IoC.Container.Register(Component.For<INotify<ApplicationRole>>()
                .ImplementedBy<Notify<ApplicationDbContext, ApplicationRole, ApplicationUser>>());

            //Register user manager
            IoC.Container.Register(Component.For<UserManager<ApplicationUser>>());

            //Dynamic data dataService
            IoC.Container.Register(Component.For<IDynamicService>()
                .ImplementedBy<DynamicService<EntitiesDbContext>>()
                .DependsOn(Dependency.OnComponent<IHttpContextAccessor, HttpContextAccessor>()));

            //Seed
            var synchronizerParams = new Dictionary<string, object> { { "context", context } };
            IoC.Container.Register(Component.For<EntitySynchronizer>().DependsOn(synchronizerParams));
            return WindsorRegistrationHelper.CreateServiceProvider(IoC.Container, services);
        }

        /// <summary>
        /// Add configured Cors
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddOriginCorsModule(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", b =>
                {
                    b.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });

            return services;
        }
        /// <summary>
        /// Use Cors
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseConfiguredCors(this IApplicationBuilder app, IConfiguration configuration)
        {
            var minutes = 1;
            if (int.TryParse(configuration["HealthCheck:Timeout"], out var minutesParsed))
                minutes = minutesParsed;
            //Enable CORS before calling app.UseMvc() and app.UseStaticFiles()

            app.UseCors("CorsPolicy")
                .UseStaticFiles()
                .UseSession()
                .UseAuthentication()
                .UseIdentityServer();

            //.UseMiddleware<HealthCheckMiddleware>(configuration["HealthCheck:Path"], TimeSpan.FromMinutes(minutes));
            return app;
        }

        /// <summary>
        /// XML Comments File path
        /// </summary>
        private static string XmlCommentsFilePath
        {
            get
            {
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var fileName = typeof(ServiceCollectionExtensions).GetTypeInfo().Assembly.GetName().Name + ".xml";
                return Path.Combine(basePath, fileName);
            }
        }
    }
}