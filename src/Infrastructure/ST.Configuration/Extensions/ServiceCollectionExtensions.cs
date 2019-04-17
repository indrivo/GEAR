using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using ST.BaseBusinessRepository;
using ST.Configuration.Seed;
using ST.Configuration.Services;
using ST.Configuration.Services.Abstraction;
using ST.DynamicEntityStorage;
using ST.DynamicEntityStorage.Abstractions;
using ST.Entities.Data;
using ST.Entities.Extensions;
using ST.Entities.Services;
using ST.Entities.Services.Abstraction;
using ST.Entities.Utils;
using ST.Files.Abstraction;
using ST.Files.Providers;
using ST.Files.Services;
using ST.Identity.Data;
using ST.Identity.Data.Groups;
using ST.Identity.Data.Permissions;
using ST.Identity.Data.UserProfiles;
using ST.Identity.Extensions;
using ST.Identity.Filters;
using ST.Identity.Services;
using ST.Identity.Services.Abstractions;
using ST.Identity.Versioning;
using ST.MPass.Gov;
using ST.MultiTenant.Services;
using ST.MultiTenant.Services.Abstractions;
using ST.Notifications.Abstraction;
using ST.Notifications.Abstractions;
using ST.Notifications.Providers;
using ST.Notifications.Services;
using ST.Procesess.Abstraction;
using ST.Procesess.Parsers;
using Swashbuckle.AspNetCore.Swagger;
using constants = ST.Identity.DbSchemaNameConstants;
using Identity_IProfileService = IdentityServer4.Services.IProfileService;
using Identity_ProfileService = ST.Identity.Services.ProfileService;

namespace ST.Configuration.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add identity server
        /// </summary>
        /// <param name="services"></param>
        /// <param name="hostingEnvironment"></param>
        /// <param name="migrationsAssembly"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityServer(this IServiceCollection services, IConfiguration configuration, IHostingEnvironment hostingEnvironment,
            string migrationsAssembly)
        {
            services.AddIdentityServer(x => x.IssuerUri = "null")
                .AddDeveloperSigningCredential()
                .AddAspNetIdentity<ApplicationUser>()
                .AddConfigurationStore(options =>
                {
                    options.DefaultSchema = constants.DEFAULT_SCHEMA;
                    options.ConfigureDbContext = builder =>
                    {
                        var connectionString = DbUtil.GetConnectionString(configuration, hostingEnvironment);
                        if (connectionString.Item1 == DbProviderType.PostgreSql)
                        {
                            builder.UseNpgsql(connectionString.Item2, opts =>
                            {
                                opts.MigrationsAssembly(migrationsAssembly);
                                opts.MigrationsHistoryTable("IdentityServerConfigurationMigrationHistory",
                                    constants.DEFAULT_SCHEMA);
                            });
                        }
                        else
                        {
                            builder.UseSqlServer(connectionString.Item2, opts =>
                            {
                                opts.MigrationsAssembly(migrationsAssembly);
                                opts.MigrationsHistoryTable("IdentityServerConfigurationMigrationHistory",
                                    constants.DEFAULT_SCHEMA);
                            });
                        }
                    };
                })
                .AddOperationalStore(options =>
                {
                    options.DefaultSchema = constants.DEFAULT_SCHEMA;
                    options.ConfigureDbContext = builder =>
                    {
                        var connectionString = DbUtil.GetConnectionString(configuration, hostingEnvironment);
                        if (connectionString.Item1 == DbProviderType.PostgreSql)
                        {
                            builder.UseNpgsql(connectionString.Item2, opts =>
                            {
                                opts.MigrationsAssembly(migrationsAssembly);
                                opts.MigrationsHistoryTable("IdentityServerConfigurationMigrationHistory",
                                    constants.DEFAULT_SCHEMA);
                            });
                        }
                        else
                        {
                            builder.UseSqlServer(connectionString.Item2, opts =>
                            {
                                opts.MigrationsAssembly(migrationsAssembly);
                                opts.MigrationsHistoryTable("IdentityServerConfigurationMigrationHistory",
                                    constants.DEFAULT_SCHEMA);
                            });
                        }
                    };
                })
                .Services.AddTransient<Identity_IProfileService, Identity_ProfileService>();
            return services;
        }

        /// <summary>
        /// Add context and identity
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="hostingEnvironment"></param>
        /// <param name="migrationsAssembly"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
        public static IServiceCollection AddDbContextAndIdentity(this IServiceCollection services,
            IConfiguration configuration, IHostingEnvironment hostingEnvironment, string migrationsAssembly, IHostingEnvironment environment)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        var connectionString = DbUtil.GetConnectionString(configuration, hostingEnvironment);
                        if (connectionString.Item1 == DbProviderType.PostgreSql)
                        {
                            options.UseNpgsql(connectionString.Item2, opts =>
                            {
                                opts.MigrationsAssembly(migrationsAssembly);
                                opts.MigrationsHistoryTable("IdentityMigrationHistory", constants.DEFAULT_SCHEMA);
                            });
                        }
                        else
                        {
                            options.UseSqlServer(connectionString.Item2, opts =>
                            {
                                opts.MigrationsAssembly(migrationsAssembly);
                                opts.MigrationsHistoryTable("IdentityMigrationHistory", constants.DEFAULT_SCHEMA);
                            });
                        }
                    })
                .AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services.AddAuthorizationBasedOnCache<ApplicationDbContext>();
            services.AddLdapAuthorization<ApplicationDbContext>();
            return services;
        }


        /// <summary>
        /// Add services relative to this application
        /// </summary>
        /// <param name="services"></param>
        /// <param name="env"></param>
        /// <param name="systemIdentifier"></param>
        /// <returns></returns>
        public static IServiceCollection AddApplicationSpecificServices(this IServiceCollection services, IHostingEnvironment env, string systemIdentifier)
        {
            services.Configure<FormOptions>(x => x.ValueCountLimit = int.MaxValue);
            services.AddTransient<Identity.Services.Abstractions.IEmailSender, EmailSender>();
            services.AddTransient<IMPassService, MPassService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IGroupRepository<ApplicationDbContext, ApplicationUser>, GroupRepository<ApplicationDbContext>>();
            services.AddTransient<IFormService, FormService<EntitiesDbContext>>();
            services.AddTransient<IMenuService, MenuService<IDynamicService>>();
            services.AddTransient<IProcessParser, ProcessParser>();
            services.AddTransient<IOrganizationService, OrganizationService>();
            services.UseCustomCacheService(env, "127.0.0.1", systemIdentifier);
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
            services.AddAuthorization();
            return services;
        }

        /// <summary>
        /// Add swagger
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration,
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
                    Title = "CORE HTTP API",
                    Version = "v1.0",
                    Description = "CORE Service HTTP API",
                    TermsOfService = "Terms Of Service"
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
            var castleContainer = new WindsorContainer();
            IoC.Container = castleContainer;
            var formsContext = services.BuildServiceProvider().GetService<EntitiesDbContext>();
            var bbRep = services.BuildServiceProvider().GetService<IBaseBusinessRepository<EntitiesDbContext>>();
            var env = services.BuildServiceProvider().GetService<IHostingEnvironment>();

            //Notifications
            var notificationConfig = new DbNotificationConfig { DbContext = formsContext };
            castleContainer.Register(Component.For<INotificationProvider>().ImplementedBy<DbNotificationProvider>()
                .DependsOn(Dependency.OnValue("config", notificationConfig)));
            castleContainer.Register(Component.For<INotificationProvider>().ImplementedBy<EmailNotificationProvider>());
            castleContainer.Register(Component.For<INotificationBuilder>().ImplementedBy<NotificationBuilder>());
            castleContainer.Register(Component.For<Notificator>().Named("Db")
                .DependsOn(Dependency.OnComponent<INotificationProvider, DbNotificationProvider>()));
            castleContainer.Register(Component.For<Notificator>().Named("Email")
                .DependsOn(Dependency.OnComponent<INotificationProvider, EmailNotificationProvider>()));
            //Register notifier 
            castleContainer.Register(Component.For<INotify<ApplicationRole>>().ImplementedBy<Notify<ApplicationDbContext, ApplicationRole, ApplicationUser>>());

            //Dynamic data dataService
            castleContainer.Register(Component.For<IDynamicService>()
                .ImplementedBy<DynamicService<EntitiesDbContext>>()
                .DependsOn(Dependency.OnComponent<IHttpContextAccessor, HttpContextAccessor>()));

            //Cache service
            castleContainer.Register(Component.For<ICacheService>()
                .ImplementedBy<CacheService>());

            //Files
            var fileConfig = new FileConfig { DbContext = formsContext, WebRootPath = env.WebRootPath };
            castleContainer.Register(Component.For<IFileProvider>().ImplementedBy<DocumentProvider>()
                .DependsOn(Dependency.OnValue("config", fileConfig)));
            castleContainer.Register(Component.For<FileManager>()
                .DependsOn(Dependency.OnComponent<IFileProvider, DocumentProvider>()));

            //Seed
            var synchronizerParams = new Dictionary<string, object> { { "context", formsContext }, { "repository", bbRep } };
            castleContainer.Register(Component.For<EntitySynchronizer>().DependsOn(synchronizerParams));
            return WindsorRegistrationHelper.CreateServiceProvider(castleContainer, services);
        }

        /// <summary>
        /// Add configured Cors
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddConfiguredCors(this IServiceCollection services)
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
            var isConfigured = configuration.GetValue<bool>("IsConfigured");
            var multiTenantTemplate = isConfigured
                ? "{tenant}/{controller=Home}/{action=Index}"
                : "{controller=Installer}/{action=Index}";

            var singleTenantTemplate = isConfigured
                ? "{controller=Home}/{action=Index}"
                : "{controller=Installer}/{action=Index}";

            app.UseCors("CorsPolicy")
                .UseStaticFiles()
                .UseSession()
                .UseAuthentication()
                .UseIdentityServer();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "multi-tenant",
                    template: singleTenantTemplate,
                    defaults: singleTenantTemplate
                    //constraints: new { tenant = new TenantRouteConstraint() }
                    );
            });

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