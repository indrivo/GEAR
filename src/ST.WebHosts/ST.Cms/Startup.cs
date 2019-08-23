using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ST.Application;
using ST.Backup.Abstractions.BackgroundServices;
using ST.Backup.Abstractions.Extensions;
using ST.Backup.PostGresSql;
using ST.Cache.Extensions;
using ST.Cms.Services.Abstractions;
using ST.Configuration.Extensions;
using ST.Configuration.Server;
using ST.Core;
using ST.Core.Extensions;
using ST.Core.Razor.Extensions;
using ST.DynamicEntityStorage.Extensions;
using ST.ECommerce.Abstractions.Extensions;
using ST.ECommerce.Abstractions.Models;
using ST.ECommerce.BaseImplementations.Data;
using ST.ECommerce.BaseImplementations.Repositories;
using ST.Email;
using ST.Email.Abstractions.Extensions;
using ST.Entities;
using ST.Entities.Abstractions.Extensions;
using ST.Entities.Data;
using ST.Entities.EntityBuilder.Postgres;
using ST.Entities.EntityBuilder.Postgres.Controls.Query;
using ST.Entities.Razor.Extensions;
using ST.Entities.Security;
using ST.Entities.Security.Abstractions.Extensions;
using ST.Entities.Security.Data;
using ST.Entities.Security.Extensions;
using ST.Files;
using ST.Files.Abstraction.Extension;
using ST.Files.Data;
using ST.Forms.Abstractions.Extensions;
using ST.Forms.Data;
using ST.Forms.Razor.Extensions;
using ST.Identity.Abstractions;
using ST.Identity.Abstractions.Extensions;
using ST.Identity.Data;
using ST.Identity.Abstractions.Models.MultiTenants;
using ST.Identity.IdentityServer4.Extensions;
using ST.Identity.LdapAuth;
using ST.Identity.LdapAuth.Abstractions.Extensions;
using ST.Identity.Permissions;
using ST.Identity.Permissions.Abstractions.Extensions;
using ST.Identity.Services;
using ST.Identity.Versioning;
using ST.Install;
using ST.Install.Abstractions.Extensions;
using ST.InternalCalendar.Razor.Extensions;
using ST.Localization;
using ST.Localization.Abstractions;
using ST.Localization.Abstractions.Extensions;
using ST.Localization.Abstractions.Models;
using ST.Localization.Services;
using ST.MPass.Gov;
using ST.Notifications;
using ST.Notifications.Abstractions.Extensions;
using ST.Notifications.Data;
using ST.Notifications.Extensions;
using ST.Notifications.Razor.Extensions;
using ST.PageRender.Abstractions.Extensions;
using ST.PageRender.Data;
using ST.PageRender.Razor.Extensions;
using ST.Procesess.Data;
using ST.Process.Razor.Extensions;
using ST.Report.Abstractions.Extensions;
using ST.Report.Dynamic;
using ST.Report.Dynamic.Data;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using ST.Entities.Security.Razor.Extensions;
using ST.Files.Box;
using ST.Files.Box.Abstraction.Extension;
using ST.Files.Box.Data;
using TreeIsoService = ST.Cms.Services.TreeIsoService;
using ST.MultiTenant.Abstractions.Extensions;
using ST.MultiTenant.Services;

namespace ST.Cms
{
	public class Startup
	{
		/// <summary>
		/// Migrations Assembly
		/// </summary>
		private static readonly string MigrationsAssembly =
			typeof(Identity.DbSchemaNameConstants).GetTypeInfo().Assembly.GetName().Name;

		/// <summary>
		/// AppSettings configuration
		/// </summary>
		private IConfiguration Configuration { get; }

		/// <summary>
		/// Hosting configuration
		/// </summary>
		private IHostingEnvironment HostingEnvironment { get; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="configuration"></param>
		/// <param name="env"></param>
		public Startup(IConfiguration configuration, IHostingEnvironment env)
		{
			Configuration = configuration;
			HostingEnvironment = env;
		}

		/// <summary>
		/// Configure cms app
		/// </summary>
		/// <param name="app"></param>
		/// <param name="env"></param>
		/// <param name="loggerFactory"></param>
		/// <param name="languages"></param>
		/// <param name="lifetime"></param>
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
			IOptionsSnapshot<LocalizationConfig> languages, IApplicationLifetime lifetime)
		{
			if (CoreApp.IsHostedOnLinux())
			{
				app.UseForwardedHeaders(new ForwardedHeadersOptions
				{
					ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
				});
			}

			if (env.IsDevelopment())
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
			app.UseUrlRewriteModule();

			//----------------------------------Origin Cors Usage-------------------------------------
			app.UseConfiguredCors(Configuration);

			//----------------------------------Use cors-------------------------------------
			app.UseAppMvc(Configuration, new Dictionary<string, Action<HttpContext>>
			{
				//rewrite root path to redirect on dynamic page, only for commerce landing page
				{
					"/", context =>
					{
						var originalPath = context.Request.Path.Value;
						context.Items["originalPath"] = originalPath;
						context.Request.Path = "/public";
					}
				}
			});

			//--------------------------------------Swagger Usage-------------------------------------
			app.UseSwagger()
				.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "ST.BPMN API v1.0"); });

			//----------------------------------Localization Usage-------------------------------------
			app.UseLocalizationModule(languages);

			//---------------------------------------SignalR Usage-------------------------------------
			app.UseSignalRModule();

			//----------------------------------Static files Usage-------------------------------------
			app.UseDefaultFiles();
			app.UseStaticFiles();

			//-------------------------Register on app events-------------------------------------
			lifetime.ApplicationStarted.Register(() => { CoreApp.OnApplicationStarted(app); });

			lifetime.RegisterAppEvents(app, nameof(MigrationsAssembly));

			if (env.IsProduction())
			{
				//Use compression
				app.UseResponseCompression();
			}
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			if (HostingEnvironment.IsProduction())
			{
				//Use compression
				services.AddResponseCompression();
			}

			//Register system config
			services.RegisterSystemConfig(Configuration);

			services.Configure<SecurityStampValidatorOptions>(options =>
			{
				// enables immediate logout, after updating the user's stat.
				options.ValidationInterval = TimeSpan.Zero;
			});

			//---------------------------------Custom cache Module-------------------------------------
			services.UseCustomCacheModule(HostingEnvironment, Configuration);

			//--------------------------------------Cors origin Module-------------------------------------
			services.AddOriginCorsModule();

			services.AddDbContext<ProcessesDbContext>(options =>
			{
				options.GetDefaultOptions(Configuration);
				options.EnableSensitiveDataLogging();
			});

			//------------------------------Identity Module-------------------------------------
			services.AddIdentityModule<ApplicationDbContext>(Configuration, HostingEnvironment, MigrationsAssembly, HostingEnvironment)
				.AddIdentityUserManager<IdentityUserManager, ApplicationUser>()
				.AddIdentityModuleStorage<ApplicationDbContext>(Configuration, MigrationsAssembly)
				.AddApplicationSpecificServices(HostingEnvironment, Configuration)
				.AddDistributedMemoryCache()
				.AddAppProvider<AppProvider>()
				.AddIdentityModuleEvents()
				.AddMvc()
				.AddJsonOptions(x => { x.SerializerSettings.DateFormatString = Settings.Date.DateFormat; });

			services.AddAuthenticationAndAuthorization(HostingEnvironment, Configuration)
				.AddAuthorizationBasedOnCache<ApplicationDbContext, PermissionService<ApplicationDbContext>>()
				.AddIdentityModuleProfileServices()
				.AddIdentityServer(Configuration, HostingEnvironment, MigrationsAssembly)
				.AddHealthChecks(checks =>
				{
					//var minutes = 1;
					//if (int.TryParse(Configuration["HealthCheck:Timeout"], out var minutesParsed))
					//	minutes = minutesParsed;

					//checks.AddSqlCheck("ApplicationDbContext-DB", connectionString.Item2, TimeSpan.FromMinutes(minutes));
				});

			//Register MPass
			services.AddMPassSigningCredentials(new MPassSigningCredentials
			{
				ServiceProviderCertificate =
					new X509Certificate2("Certificates/samplempass.pfx", "qN6n31IT86684JO"),
				IdentityProviderCertificate = new X509Certificate2("Certificates/testmpass.cer")
			});

			//---------------------------------Api version Module-------------------------------------
			services.AddApiVersioning(options =>
			{
				options.ReportApiVersions = true;
				options.AssumeDefaultVersionWhenUnspecified = true;
				options.DefaultApiVersion = new ApiVersion(1, 0);
				options.ErrorResponses = new UnsupportedApiVersionErrorResponseProvider();
			});
			//---------------------------------------Entity Module-------------------------------------
			services.AddEntityModule<EntitiesDbContext, EntityRepository>()
				.AddEntityModuleQueryBuilders<NpgTableQueryBuilder, NpgEntityQueryBuilder, NpgTablesService>()
				.AddEntityModuleStorage<EntitiesDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				})
				.AddEntityModuleEvents()
				.AddEntityRazorUIModule();

			//------------------------------Entity Security Module-------------------------------------
			services.AddEntityRoleAccessModule<EntityRoleAccessManager<EntitySecurityDbContext, ApplicationDbContext>>()
				.AddEntityModuleSecurityStorage<EntitySecurityDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				})
				.AddEntitySecurityRazorUIModule();

			//---------------------------Multi Tenant Module-------------------------------------
			services.AddTenantModule<OrganizationService, Tenant>();

			//---------------------------Dynamic repository Module-------------------------------------
			services.AddDynamicDataProviderModule<EntitiesDbContext>();

			//--------------------------------------SignalR Module-------------------------------------
			services.AddSignalRModule<ApplicationDbContext, ApplicationUser, ApplicationRole>();

			//--------------------------Notification subscriptions-------------------------------------
			services.AddNotificationSubscriptionModule<NotificationSubscriptionRepository>()
				.AddNotificationModuleEvents()
				.AddNotificationSubscriptionModuleStorage<NotificationDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				})
				.AddNotificationRazorUIModule();

			//---------------------------Background services ------------------------------------------
			//services.AddHostedService<HostedTimeService>();

			//--------------------------------------Swagger Module-------------------------------------
			services.AddSwaggerModule(Configuration, HostingEnvironment);

			//---------------------------------Localization Module-------------------------------------
			services.AddLocalizationModule<LocalizationService, YandexTranslationProvider>(new TranslationModuleOptions
			{
				Configuration = Configuration,
				LocalizationProvider = LocalizationProvider.Yandex
			});

			//------------------------------Database backup Module-------------------------------------
			services
				.RegisterDatabaseBackupRunnerModule<BackupTimeService<PostGreSqlBackupSettings>,
					PostGreSqlBackupSettings, PostGreBackupService>(Configuration);

			//------------------------------------Page render Module-------------------------------------
			services.AddPageRenderUiModule();

			//------------------------------------Processes Module-------------------------------------
			services.AddProcessesModule();

			//------------------------------------File Module-------------------------------------
			services
				.AddFileModule<FileManager<FileDbContext>>()
				.AddFileModuleStorage<FileDbContext>(options =>
			{
				options.GetDefaultOptions(Configuration);
				options.EnableSensitiveDataLogging();
			});

			services
				.AddFileBoxModule<FileBoxManager<FileBoxDbContext>>()
				.AddFileBoxModuleStorage<FileBoxDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				});

			//----------------------------Internal calendar Module-------------------------------------
			services.AddInternalCalendarModule();

			//-----------------------------------------Form Module-------------------------------------
			services.AddFormModule<FormDbContext>()
				.AddFormModuleStorage<FormDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				})
				.AddFormStaticFilesModule();

			//-----------------------------------------Page Module-------------------------------------
			services.AddPageModule<DynamicPagesDbContext>()
				.AddPageModuleStorage<DynamicPagesDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				});


			//---------------------------------------Report Module-------------------------------------
			services.AddDynamicReportModule<DynamicReportsService<DynamicReportDbContext>>()
				.AddDynamicReportModuleStorage<DynamicReportDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				});


			services.AddInstallerModule<SyncInstaller>();

			//----------------------------------------Email Module-------------------------------------
			services.AddEmailModule<EmailSender>()
				.BindEmailSettings(Configuration);

			if (CoreApp.IsHostedOnLinux())
			{
				services.Configure<ForwardedHeadersOptions>(options =>
				{
					options.KnownProxies.Add(IPAddress.Parse("185.131.222.95"));
				});
			}

			//----------------------------------------Ldap Module-------------------------------------
			services
				.AddIdentityLdapModule<ApplicationUser, LdapService<ApplicationUser>, LdapUserManager<ApplicationUser>>(
					Configuration);

			//-------------------------------------Commerce module-------------------------------------
			services.RegisterCommerceModule<CommerceDbContext>()
				.RegisterCommerceProductRepository<ProductRepository, Product>()
				.RegisterCommerceStorage<CommerceDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				})
				.RegisterCommerceEvents();


			//------------------------------------------Custom ISO-------------------------------------
			services.AddTransient<ITreeIsoService, TreeIsoService>();

			//--------------------------Custom dependency injection-------------------------------------
			return services.AddWindsorContainers();
		}
	}
}