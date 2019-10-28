#region Usings

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using GR.Application;
using GR.Backup.Abstractions.BackgroundServices;
using GR.Backup.Abstractions.Extensions;
using GR.Backup.PostGresSql;
using GR.Cache.Extensions;
using GR.Cms.Services.Abstractions;
using GR.Core;
using GR.Core.Extensions;
using GR.Core.Razor.Extensions;
using GR.DynamicEntityStorage.Extensions;
using GR.ECommerce.Abstractions.Extensions;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.BaseImplementations.Data;
using GR.ECommerce.BaseImplementations.Repositories;
using GR.Email;
using GR.Email.Abstractions.Extensions;
using GR.Entities;
using GR.Entities.Abstractions.Extensions;
using GR.Entities.Data;
using GR.Entities.EntityBuilder.Postgres;
using GR.Entities.EntityBuilder.Postgres.Controls.Query;
using GR.Entities.Razor.Extensions;
using GR.Entities.Security;
using GR.Entities.Security.Abstractions.Extensions;
using GR.Entities.Security.Data;
using GR.Files;
using GR.Files.Abstraction.Extension;
using GR.Files.Data;
using GR.Forms.Abstractions.Extensions;
using GR.Forms.Data;
using GR.Forms.Razor.Extensions;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Extensions;
using GR.Identity.Data;
using GR.Identity.Abstractions.Models.MultiTenants;
using GR.Identity.IdentityServer4.Extensions;
using GR.Identity.LdapAuth;
using GR.Identity.LdapAuth.Abstractions.Extensions;
using GR.Identity.Permissions;
using GR.Identity.Permissions.Abstractions.Extensions;
using GR.Identity.Services;
using GR.Identity.Versioning;
using GR.Install.Abstractions.Extensions;
using GR.Localization.Abstractions;
using GR.Localization.Abstractions.Extensions;
using GR.Localization.Abstractions.Models;
using ST.MPass.Gov;
using GR.Notifications;
using GR.Notifications.Abstractions.Extensions;
using GR.Notifications.Data;
using GR.Notifications.Extensions;
using GR.Notifications.Razor.Extensions;
using GR.PageRender.Abstractions.Extensions;
using GR.PageRender.Data;
using GR.PageRender.Razor.Extensions;
using GR.Procesess.Data;
using GR.Process.Razor.Extensions;
using GR.Report.Abstractions.Extensions;
using GR.Report.Dynamic;
using GR.Report.Dynamic.Data;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using GR.Application.Middleware.Extensions;
using GR.Application.Middleware.Server;
using GR.Audit;
using GR.Audit.Abstractions.Extensions;
using GR.Calendar;
using GR.Calendar.Abstractions.Extensions;
using GR.Calendar.Abstractions.ExternalProviders;
using GR.Calendar.Abstractions.ExternalProviders.Extensions;
using GR.Calendar.Data;
using GR.Calendar.Providers.Google.Extensions;
using GR.Calendar.Providers.Outlook.Extensions;
using GR.Calendar.Razor.Extensions;
using GR.Dashboard;
using GR.Dashboard.Abstractions;
using GR.Dashboard.Abstractions.Extensions;
using GR.Dashboard.Abstractions.Models.WidgetTypes;
using GR.Dashboard.Data;
using GR.Dashboard.Razor.Extensions;
using GR.Dashboard.Renders;
using GR.Email.Razor.Extensions;
using GR.Entities.Security.Razor.Extensions;
using GR.Files.Box;
using GR.Files.Box.Abstraction.Extension;
using GR.Files.Box.Data;
using TreeIsoService = GR.Cms.Services.TreeIsoService;
using GR.MultiTenant.Abstractions.Extensions;
using GR.MultiTenant.Razor.Extensions;
using GR.MultiTenant.Services;
using GR.Report.Dynamic.Razor.Extensions;
using GR.TaskManager.Abstractions.Extensions;
using GR.TaskManager.Data;
using GR.TaskManager.Razor.Extensions;
using GR.TaskManager.Services;
using GR.Calendar.NetCore.Api.GraphQL.Extensions;
using GR.ECommerce.Paypal;
using GR.Entities.Extensions;
using GR.Localization;
using GR.Paypal.Abstractions.Extensions;
using GR.Paypal.Razor.Extensions;

#endregion

namespace GR.Cms
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
		/// <param name="languages"></param>
		/// <param name="lifetime"></param>
		public void Configure(IApplicationBuilder app, IHostingEnvironment env,
			IOptionsSnapshot<LocalizationConfig> languages, IApplicationLifetime lifetime)
		{
			if (GearApplication.IsHostedOnLinux())
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

			app.UseCalendarGrapHQL();

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
				.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "GR.BPMN API v1.0"); });

			//----------------------------------Localization Usage-------------------------------------
			app.UseLocalizationModule(languages);

			//---------------------------------------SignalR Usage-------------------------------------
			app.UseSignalRModule();

			//----------------------------------Static files Usage-------------------------------------
			app.UseDefaultFiles();
			app.UseStaticFiles();

			//-------------------------Register on app events-------------------------------------
			lifetime.ApplicationStarted.Register(() => { GearApplication.ApplicationStarted(app); });

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
			services.AddHttpClient();
			//Register system config
			services.RegisterSystemConfig(Configuration);

			services.Configure<SecurityStampValidatorOptions>(options =>
			{
				// enables immediate logout, after updating the user's stat.
				options.ValidationInterval = TimeSpan.Zero;
			});

			//---------------------------------Custom cache Module-------------------------------------
			services.AddCacheModule(HostingEnvironment, Configuration);

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
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
				.AddJsonOptions(x => { x.SerializerSettings.DateFormatString = GearSettings.Date.DateFormat; });

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
				.RegisterEntityBuilderJob()
				.AddEntityRazorUIModule();

			//------------------------------Entity Security Module-------------------------------------
			services.AddEntityRoleAccessModule<EntityRoleAccessManager<EntitySecurityDbContext, ApplicationDbContext>>()
				.AddEntityModuleSecurityStorage<EntitySecurityDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				})
				.AddEntitySecurityRazorUIModule();

			//----------------------------------------Audit Module-------------------------------------
			services.AddAuditModule<AuditManager>();

			//---------------------------Dynamic repository Module-------------------------------------
			services.AddDynamicDataProviderModule<EntitiesDbContext>();

			//------------------------------------Dashboard Module-------------------------------------
			services.AddDashboardModule<DashboardService, WidgetGroupRepository, WidgetService>()
				.AddDashboardModuleStorage<DashBoardDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				})
				.RegisterDashboardEvents()
				.AddDashboardRazorUIModule()
				.AddDashboardRenderServices(new Dictionary<Type, Type>
				{
					{typeof(IWidgetRenderer<ReportWidget>), typeof(ReportWidgetRender)},
					{typeof(IWidgetRenderer<CustomWidget>), typeof(CustomWidgetRender)},
				})
				.RegisterProgramAssembly(typeof(Program));

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

			//---------------------------------Localization Module-------------------------------------
			services.AddLocalizationModule<LocalizationService, YandexTranslationProvider, JsonStringLocalizer>(new TranslationModuleOptions
			{
				Configuration = Configuration,
				LocalizationProvider = LocalizationProvider.Yandex
			});

			//------------------------------Database backup Module-------------------------------------
			services.RegisterDatabaseBackupRunnerModule<BackupTimeService<PostGreSqlBackupSettings>,
					PostGreSqlBackupSettings, PostGreBackupService>(Configuration);

			//------------------------------------Page render Module-------------------------------------
			services.AddPageRenderUiModule();

			//------------------------------------Processes Module-------------------------------------
			services.AddProcessesModule();

			//------------------------------------Calendar Module-------------------------------------
			services.AddCalendarModule<CalendarManager>()
				.AddCalendarModuleStorage<CalendarDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				})
				.AddCalendarRazorUIModule()
				.SetSerializationFormatSettings(settings =>
				{
					settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
				})
				.AddCalendarRuntimeEvents()
				.RegisterSyncOnExternalCalendars()
				.RegisterTokenProvider<CalendarExternalTokenProvider>()
				.RegisterCalendarUserPreferencesProvider<CalendarUserSettingsService>()
				.RegisterGoogleCalendarProvider()
				.RegisterOutlookCalendarProvider(options =>
				{
					options.ClientId = "d883c965-781c-4520-b7e7-83543eb92b4a";
					options.ClientSecretId = "./7v5Ns0cT@K?BdD85J/r1MkE1rlPran";
					options.TenantId = "f24a7cfa-3648-4303-b392-37bb02d09d28";
				})
				.AddCalendarGraphQLApi();

			//------------------------------------File Module-------------------------------------
			services.AddFileModule<FileManager<FileDbContext>>()
				.AddFileModuleStorage<FileDbContext>(options =>
			{
				options.GetDefaultOptions(Configuration);
				options.EnableSensitiveDataLogging();
			}, Configuration);

			services
				.AddFileBoxModule<FileBoxManager<FileBoxDbContext>>()
				.AddFileBoxModuleStorage<FileBoxDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				}, Configuration);
			//------------------------------------Task Module-------------------------------------
			services
				.AddTaskModule<TaskManager.Services.TaskManager, TaskManagerNotificationService>()
				.AddTaskModuleStorage<TaskManagerDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				})
				.AddTaskManagerRazorUIModule();

			//-----------------------------------------Form Module-------------------------------------
			services.AddFormModule<FormDbContext>()
				.AddFormModuleStorage<FormDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				})
				.AddFormStaticFilesModule();

			//-----------------------------------------Page Module-------------------------------------
			services.AddPageModule()
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
				})
				.AddReportUIModule();


			services.AddInstallerModule();

			//----------------------------------------Email Module-------------------------------------
			services.AddEmailModule<EmailSender>()
				.AddEmailRazorUIModule()
				.BindEmailSettings(Configuration);

			if (GearApplication.IsHostedOnLinux())
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
				.RegisterPaypalProvider<PaypalPaymentService>()
				.RegisterPaypalRazorProvider(Configuration)
				.RegisterCommerceEvents();

			//---------------------------------Multi Tenant Module-------------------------------------
			services.AddTenantModule<OrganizationService, Tenant>()
				.AddMultiTenantRazorUIModule();


			//--------------------------------------Swagger Module-------------------------------------
			services.AddSwaggerModule(Configuration, HostingEnvironment);


			//------------------------------------------Custom ISO-------------------------------------
			services.AddTransient<ITreeIsoService, TreeIsoService>();

			//--------------------------Custom dependency injection-------------------------------------
			return services.AddWindsorContainers();
		}
	}
}