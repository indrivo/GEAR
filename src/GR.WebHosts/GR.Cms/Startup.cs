#region Usings

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GR.Backup.Abstractions.BackgroundServices;
using GR.Backup.Abstractions.Extensions;
using GR.Backup.PostGresSql;
using GR.Cms.Services.Abstractions;
using GR.Core.Extensions;
using GR.DynamicEntityStorage.Extensions;
using GR.ECommerce.Abstractions.Extensions;
using GR.ECommerce.Abstractions.Models;
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
using GR.Identity.LdapAuth;
using GR.Identity.LdapAuth.Abstractions.Extensions;
using GR.Identity.Permissions;
using GR.Identity.Services;
using GR.Install.Abstractions.Extensions;
using GR.Localization.Abstractions;
using GR.Localization.Abstractions.Extensions;
using GR.Localization.Abstractions.Models;
using ST.MPass.Gov;
using GR.Notifications;
using GR.Notifications.Abstractions.Extensions;
using GR.Notifications.Data;
using GR.Notifications.Razor.Extensions;
using GR.PageRender.Abstractions.Extensions;
using GR.PageRender.Data;
using GR.PageRender.Razor.Extensions;
using GR.Procesess.Data;
using GR.Process.Razor.Extensions;
using GR.Report.Abstractions.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using GR.Application.Middleware.Extensions;
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
using GR.Documents;
using GR.DynamicEntityStorage.Abstractions;
using GR.ECommerce.BaseImplementations.Data;
using GR.ECommerce.Payments.Abstractions.Extensions;
using GR.ECommerce.Products.Services;
using GR.ECommerce.Razor.Extensions;
using GR.Entities.Extensions;
using GR.Localization;
using GR.Orders;
using GR.Orders.Abstractions.Models;
using GR.PageRender;
using GR.Paypal;
using GR.Paypal.Abstractions.Extensions;
using GR.MobilPay;
using GR.MobilPay.Abstractions.Extensions;
using GR.MobilPay.Razor.Extensions;
using GR.Orders.Abstractions.Extensions;
using GR.Paypal.Razor.Extensions;
using GR.Subscriptions.Abstractions.Models;
using GR.Subscriptions;
using GR.Subscriptions.Abstractions.Extensions;
using GR.Documents.Abstractions.Extensions;
using GR.Documents.Abstractions.Models;
using GR.Documents.Data;
using GR.Identity.IdentityServer4.Extensions;
using GR.Identity.LdapAuth.Abstractions.Models;
using GR.Identity.Permissions.Abstractions.Extensions;
using GR.Report.Dynamic;
using GR.Report.Dynamic.Data;
using GR.Subscriptions.BackgroundServices;
using GR.WebApplication.Extensions;
using GR.WorkFlows;
using GR.WorkFlows.Abstractions.Models;
using GR.WorkFlows.Abstractions.Extensions;
using GR.WorkFlows.Data;
using GR.WorkFlows.Razor.Extensions;

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
		public void Configure(IApplicationBuilder app)
		{
			app.UseGearWebApp(config =>
				{
					config.HostingEnvironment = HostingEnvironment;
					config.Configuration = Configuration;
					config.CustomMapRules = new Dictionary<string, Action<HttpContext>>
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
					};
				})
				//----------------------------------Calendar graphQl--------------------------------------
				.UseCalendarGraphQl();
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public IServiceProvider ConfigureServices(IServiceCollection services) =>
			services.RegisterGearWebApp(config =>
		{
			config.Configuration = Configuration;
			config.HostingEnvironment = HostingEnvironment;
			config.CacheConfiguration.UseInMemoryCache = true;

			//------------------------------Identity Module-------------------------------------
			config.GearServices.AddIdentityModule<ApplicationDbContext>(Configuration, HostingEnvironment,
					MigrationsAssembly, HostingEnvironment)
				.AddIdentityUserManager<IdentityUserManager, GearUser>()
				.AddIdentityModuleStorage<ApplicationDbContext>(Configuration, MigrationsAssembly)
				.AddApplicationSpecificServices(HostingEnvironment, Configuration)
				.AddAppProvider<AppProvider>()
				.AddUserAddressService<UserAddressService>()
				.AddIdentityModuleEvents();

			config.GearServices.AddAuthenticationAndAuthorization(HostingEnvironment, Configuration)
				.AddPermissionService<PermissionService<ApplicationDbContext>>()
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
			config.GearServices.AddMPassSigningCredentials(new MPassSigningCredentials
			{
				ServiceProviderCertificate =
					new X509Certificate2("Certificates/samplempass.pfx", "qN6n31IT86684JO"),
				IdentityProviderCertificate = new X509Certificate2("Certificates/testmpass.cer")
			});

			//---------------------------------------Entity Module-------------------------------------
			config.GearServices.AddEntityModule<EntitiesDbContext, EntityRepository>()
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
			config.GearServices.AddEntityRoleAccessModule<EntityRoleAccessManager<EntitySecurityDbContext, ApplicationDbContext>>()
				.AddEntityModuleSecurityStorage<EntitySecurityDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				})
				.AddEntitySecurityRazorUIModule();

			//----------------------------------------Audit Module-------------------------------------
			config.GearServices.AddAuditModule<AuditManager>();

			//---------------------------Dynamic repository Module-------------------------------------
			config.GearServices.AddDynamicDataProviderModule<EntitiesDbContext>();

			//------------------------------------Dashboard Module-------------------------------------
			config.GearServices.AddDashboardModule<DashboardService, WidgetGroupRepository, WidgetService>()
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

			//--------------------------Notification subscriptions-------------------------------------
			config.GearServices.AddNotificationSubscriptionModule<NotificationSubscriptionRepository>()
				.AddNotificationModuleEvents()
				.AddNotificationSubscriptionModuleStorage<NotificationDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				})
				.AddNotificationRazorUIModule();

			//---------------------------------Localization Module-------------------------------------
			config.GearServices.AddLocalizationModule<LocalizationService, YandexTranslationProvider, JsonStringLocalizer>(new TranslationModuleOptions
			{
				Configuration = Configuration,
				LocalizationProvider = LocalizationProvider.Yandex
			});

			//------------------------------Database backup Module-------------------------------------
			config.GearServices.RegisterDatabaseBackupRunnerModule<BackupTimeService<PostGreSqlBackupSettings>,
					PostGreSqlBackupSettings, PostGreBackupService>(Configuration);

			//------------------------------------Processes Module-------------------------------------
			config.GearServices.AddProcessesModule()
			.AddDbContext<ProcessesDbContext>(options =>
			{
				options.GetDefaultOptions(Configuration);
				options.EnableSensitiveDataLogging();
			});

			//------------------------------------Calendar Module-------------------------------------
			config.GearServices.AddCalendarModule<CalendarManager>()
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
				.AddCalendarGraphQlApi();

			//------------------------------------File Module-------------------------------------
			config.GearServices.AddFileModule<FileManager<FileDbContext>>()
				.AddFileModuleStorage<FileDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				}, Configuration);

			config.GearServices
				.AddFileBoxModule<FileBoxManager<FileBoxDbContext>>()
				.AddFileBoxModuleStorage<FileBoxDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				}, Configuration);
			//------------------------------------Task Module-------------------------------------
			config.GearServices.AddTaskModule<TaskManager.Services.TaskManager, TaskManagerNotificationService>()
				.AddTaskModuleStorage<TaskManagerDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				})
				.AddTaskManagerRazorUIModule();

			//-----------------------------------------Form Module-------------------------------------
			config.GearServices.AddFormModule<FormDbContext>()
				.AddFormModuleStorage<FormDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				})
				.AddFormStaticFilesModule();

			//-----------------------------------------Page Module-------------------------------------
			config.GearServices.AddPageModule()
				.AddPageModuleStorage<DynamicPagesDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				})
				.AddPageRenderUIModule<PageRender.PageRender>()
				.AddMenuService<MenuService<IDynamicService>>()
				.RegisterViewModelService<ViewModelService>()
				.AddPageAclService<PageAclService>();


			//---------------------------------------Report Module-------------------------------------
			config.GearServices.AddDynamicReportModule<DynamicReportsService<DynamicReportDbContext>>()
				.AddDynamicReportModuleStorage<DynamicReportDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				})
				.AddReportUIModule();


			config.GearServices.AddInstallerModule();

			//----------------------------------------Email Module-------------------------------------
			config.GearServices.AddEmailModule<EmailSender>()
				.AddEmailRazorUIModule()
				.BindEmailSettings(Configuration);

			//----------------------------------------Ldap Module-------------------------------------
			config.GearServices
				.AddIdentityLdapModule<LdapUser, LdapService<LdapUser>, LdapUserManager<LdapUser>>(
					Configuration);

			//-------------------------------------Commerce module-------------------------------------
			config.GearServices.RegisterCommerceModule<CommerceDbContext>()
				.RegisterCommerceProductRepository<ProductService, Product>()
				.RegisterCommerceStorage<CommerceDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				})
				.RegisterPaypalProvider<PaypalPaymentMethodService>()
				.RegisterMobilPayProvider<MobilPayPaymentMethodService>()
				.RegisterPaypalRazorProvider(Configuration)
				.RegisterProductOrderServices<Order, OrderProductService>()
				.RegisterSubscriptionServices<Subscription, SubscriptionService>()
				.RegisterPayments<PaymentService>()
				.RegisterCartService<CartService>()
				.RegisterOrdersStorage<CommerceDbContext>()
				.RegisterSubscriptionStorage<CommerceDbContext>()
				.RegisterPaymentStorage<CommerceDbContext>()
				.RegisterCommerceEvents()
				.RegisterOrderEvents()
				.RegisterSubscriptionEvents()
				.RegisterSubscriptionRules()
				.RegisterBackgroundService<SubscriptionValidationBackgroundService>()
				.RegisterMobilPayRazorProvider(Configuration)
				.AddCommerceRazorUIModule();

			//---------------------------------Multi Tenant Module-------------------------------------
			config.GearServices.AddTenantModule<OrganizationService, Tenant>()
				.AddMultiTenantRazorUIModule();

			//------------------------------------------Custom ISO-------------------------------------
			config.GearServices.AddTransient<ITreeIsoService, TreeIsoService>();

			//-------------------------------------Workflow module-------------------------------------
			config.GearServices.AddWorkFlowModule<WorkFlow, WorkFlowCreatorService, WorkFlowExecutorService>()
				.AddWorkflowModuleStorage<WorkFlowsDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				})
				.AddWorkflowRazorModule()
				.RegisterWorkFlowContract(nameof(DocumentVersion), null);


			//------------------------------------ Documents Module -----------------------------------

			config.GearServices.RegisterDocumentStorage<DocumentsDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				})
				.RegisterDocumentTypeServices<DocumentTypeService>()
				.RegisterDocumentCategoryServices<DocumentCategoryService>()
				.RegisterDocumentServices<DocumentWithWorkflowService>();
		});
	}
}