using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.MsDependencyInjection;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Shared.Core.Filters;
using Shared.Core.Versioning;
using ST.BaseBusinessRepository;
using ST.CORE.Extensions.Installer;
using ST.CORE.Models.LocalizationViewModels;
using ST.CORE.Services;
using ST.CORE.Services.Abstraction;
using ST.Entities.Data;
using ST.Entities.Services;
using ST.Entities.Services.Abstraction;
using ST.Entities.Utils;
using ST.Files.Abstraction;
using ST.Files.Providers;
using ST.Files.Services;
using ST.Identity;
using ST.Identity.Data;
using ST.Identity.Data.Groups;
using ST.Identity.Data.Permissions;
using ST.Identity.Data.UserProfiles;
using ST.Identity.Extensions;
using ST.Localization;
using ST.MPass.Gov;
using ST.Notifications.Abstraction;
using ST.Notifications.Providers;
using ST.Notifications.Services;
using ST.Procesess.Abstraction;
using ST.Procesess.Parsers;
using Swashbuckle.AspNetCore.Swagger;
using constants = ST.Identity.DbSchemaNameConstants;

namespace ST.CORE.Extensions
{
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// Add identity server
		/// </summary>
		/// <param name="services"></param>
		/// <param name="connectionString"></param>
		/// <param name="migrationsAssembly"></param>
		/// <returns></returns>
		public static IServiceCollection AddIdentityServer(this IServiceCollection services, (DbProviderType, string) connectionString,
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
				.Services.AddTransient<IProfileService, ProfileService>();
			return services;
		}

		/// <summary>
		/// Add context and identity
		/// </summary>
		/// <param name="services"></param>
		/// <param name="connectionString"></param>
		/// <param name="migrationsAssembly"></param>
		/// <param name="environment"></param>
		/// <returns></returns>
		public static IServiceCollection AddDbContextAndIdentity(this IServiceCollection services,
			(DbProviderType, string) connectionString, string migrationsAssembly, IHostingEnvironment environment)
		{
			services.AddDbContext<ApplicationDbContext>(options =>
					{
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
			services.AddAuthorizationBasedOnCache();
			services.AddLdapAuthorization();
			return services;
		}


		/// <summary>
		/// Add services relative to this application
		/// </summary>
		/// <param name="services"></param>
		/// <param name="env"></param>
		/// <returns></returns>
		public static IServiceCollection AddApplicationSpecificServices(this IServiceCollection services, IHostingEnvironment env)
		{
			services.Configure<FormOptions>(x => x.ValueCountLimit = int.MaxValue);
			services.AddTransient<IEmailSender, EmailSender>();
			services.AddTransient<IMPassService, MPassService>();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddTransient<IGroupRepository<ApplicationDbContext, ApplicationUser>, GroupRepository>();
			services.AddTransient<IFormService, FormService>();
			services.AddTransient<ILocalizationService, LocalizationService>();
			services.AddTransient<IPageRender, PageRender>();
			services.AddTransient<IMenuService, MenuService>();
			services.AddTransient<IProcessParser, ProcessParser>();
			services.UseCustomCacheService(env);
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
		/// Add localization
		/// </summary>
		/// <param name="services"></param>
		/// <param name="configuration"></param>
		/// <returns></returns>
		public static IServiceCollection AddStLocalization(this IServiceCollection services,
			IConfiguration configuration)
		{
			services.AddTransient<IStringLocalizer, JsonStringLocalizer>();
			services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
			services.Configure<LocalizationConfigModel>(configuration.GetSection(nameof(LocalizationConfig)));
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			services.AddSession(opts =>
			{
				opts.IdleTimeout = TimeSpan.FromDays(1);
				opts.Cookie.HttpOnly = true;
			});
			return services;
		}

		/// <summary>
		/// Use Localization
		/// </summary>
		/// <param name="app"></param>
		/// <param name="language"></param>
		/// <returns></returns>
		public static IApplicationBuilder UseLocalization(this IApplicationBuilder app,
			IOptionsSnapshot<LocalizationConfig> language)
		{
			var supportedCultures = language.Value.Languages.Select(str => new CultureInfo(str.Identifier)).ToList();
			var opts = new RequestLocalizationOptions
			{
				DefaultRequestCulture = new RequestCulture("en"),
				SupportedCultures = supportedCultures,
				SupportedUICultures = supportedCultures
			};
			app.UseRequestLocalization(opts);
			app.UseSession();
			var locMon = app.ApplicationServices.GetRequiredService<IOptionsMonitor<LocalizationConfigModel>>();
			locMon.OnChange(locConfig =>
			{
				var languages = locConfig.Languages.Select(lStr => new CultureInfo(lStr.Identifier)).ToList();
				var reqLoc = app.ApplicationServices.GetRequiredService<IOptionsSnapshot<RequestLocalizationOptions>>();

				reqLoc.Value.SupportedCultures = languages;
				reqLoc.Value.SupportedUICultures = languages;
			});
			return app;
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
			//var conf = services.BuildServiceProvider().GetService<IConfiguration>();
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
			var isConfigurated = configuration.GetValue<bool>("IsConfigurated");
			var multiTenantTemplate = isConfigurated
				? "{tenant}/{controller=Home}/{action=Index}"
				: "{controller=Installer}/{action=Index}";

			var singleTenantTemplate = isConfigurated
				? "{controller=Home}/{action=Index}"
				: "{controller=Installer}/{action=Index}";

			app.UseCors("CorsPolicy")
				.UseStaticFiles()
				.UseSession()
				.UseAuthentication()
				.UseIdentityServer()
				.UseMvc(routes =>
				{
					//routes.MapRoute(
					//	name: "multi-tenant",
					//	template: multiTenantTemplate,
					//	defaults: multiTenantTemplate,
					//	constraints: new { tenant = new TenantRouteConstraint() }
					//	);

					routes.MapRoute(
						name: "single-tenant",
						template: singleTenantTemplate
					);
				})
				.UseMiddleware<HealthCheckMiddleware>(configuration["HealthCheck:Path"], TimeSpan.FromMinutes(minutes));
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
				var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
				return Path.Combine(basePath, fileName);
			}
		}
	}
}