#region Usings

using GR.Audit;
using GR.Audit.Abstractions.Extensions;
using GR.Backup.Abstractions.BackgroundServices;
using GR.Backup.Abstractions.Extensions;
using GR.Backup.PostGresSql;
using GR.Calendar;
using GR.Calendar.Abstractions.Extensions;
using GR.Calendar.Abstractions.ExternalProviders;
using GR.Calendar.Abstractions.ExternalProviders.Extensions;
using GR.Calendar.Data;
using GR.Calendar.NetCore.Api.GraphQL.Extensions;
using GR.Calendar.Providers.Google.Extensions;
using GR.Calendar.Providers.Outlook.Extensions;
using GR.Calendar.Razor.Extensions;
using GR.Core.Extensions;
using GR.Dashboard;
using GR.Dashboard.Abstractions;
using GR.Dashboard.Abstractions.Extensions;
using GR.Dashboard.Abstractions.Models.WidgetTypes;
using GR.Dashboard.Data;
using GR.Dashboard.Razor.Extensions;
using GR.Dashboard.Renders;
using GR.Documents;
using GR.Documents.Abstractions.Extensions;
using GR.Documents.Abstractions.Models;
using GR.Documents.Data;
using GR.DynamicEntityStorage.Extensions;
using GR.ECommerce.Abstractions.Extensions;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.BaseImplementations.Data;
using GR.ECommerce.Payments.Abstractions.Extensions;
using GR.ECommerce.Products.Services;
using GR.ECommerce.Razor.Extensions;
using GR.Email;
using GR.Email.Abstractions.Extensions;
using GR.Email.Razor.Extensions;
using GR.Entities;
using GR.Entities.Abstractions.Extensions;
using GR.Entities.Data;
using GR.Entities.EntityBuilder.Postgres;
using GR.Entities.EntityBuilder.Postgres.Controls.Query;
using GR.Entities.Extensions;
using GR.Entities.Razor.Extensions;
using GR.Entities.Security;
using GR.Entities.Security.Abstractions.Extensions;
using GR.Entities.Security.Data;
using GR.Entities.Security.Razor.Extensions;
using GR.Files;
using GR.Files.Abstraction.Extension;
using GR.Files.Box;
using GR.Files.Box.Abstraction.Extension;
using GR.Files.Box.Data;
using GR.Files.Data;
using GR.Forms.Abstractions.Extensions;
using GR.Forms.Data;
using GR.Forms.Razor.Extensions;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Extensions;
using GR.Identity.Abstractions.Models.MultiTenants;
using GR.Identity.Data;
using GR.Identity.LdapAuth;
using GR.Identity.LdapAuth.Abstractions.Extensions;
using GR.Identity.LdapAuth.Abstractions.Models;
using GR.Identity.Permissions;
using GR.Identity.Permissions.Abstractions.Extensions;
using GR.Install;
using GR.Install.Abstractions.Extensions;
using GR.Localization;
using GR.Localization.Abstractions.Extensions;
using GR.MobilPay;
using GR.MobilPay.Abstractions.Extensions;
using GR.MobilPay.Razor.Extensions;
using GR.MultiTenant.Abstractions.Extensions;
using GR.MultiTenant.Razor.Extensions;
using GR.MultiTenant.Services;
using GR.Notifications;
using GR.Notifications.Abstractions.Extensions;
using GR.Notifications.Data;
using GR.Notifications.Razor.Extensions;
using GR.Orders;
using GR.Orders.Abstractions.Extensions;
using GR.Orders.Abstractions.Models;
using GR.PageRender;
using GR.PageRender.Abstractions.Extensions;
using GR.PageRender.Data;
using GR.PageRender.Razor.Extensions;
using GR.Paypal;
using GR.Paypal.Abstractions.Extensions;
using GR.Paypal.Razor.Extensions;
using GR.Report.Abstractions.Extensions;
using GR.Report.Dynamic;
using GR.Report.Dynamic.Data;
using GR.Report.Dynamic.Razor.Extensions;
using GR.Subscriptions;
using GR.Subscriptions.Abstractions.Extensions;
using GR.Subscriptions.Abstractions.Models;
using GR.Subscriptions.BackgroundServices;
using GR.TaskManager.Abstractions.Extensions;
using GR.TaskManager.Data;
using GR.TaskManager.Razor.Extensions;
using GR.TaskManager.Services;
using GR.WebApplication.Extensions;
using GR.WebApplication.Helpers;
using GR.WorkFlows;
using GR.WorkFlows.Abstractions.Extensions;
using GR.WorkFlows.Abstractions.Models;
using GR.WorkFlows.Data;
using GR.WorkFlows.Razor.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using GR.Backup.Razor.Extensions;
using GR.Braintree;
using GR.Braintree.Abstractions.Extensions;
using GR.Braintree.Razor.Extensions;
using GR.Calendar.Abstractions.Helpers;
using GR.Forms;
using GR.Localization.Razor.Extensions;
using GR.Notifications.Services;
using GR.UI.Menu;
using GR.UI.Menu.Abstractions.Extensions;
using GR.UI.Menu.Data;
using GR.Documents.Razor.Extensions;
using GR.Entities.Abstractions.Helpers;
using GR.Files.Razor.Extensions;
using GR.Forms.Abstractions.Helpers;
using GR.Identity;
using GR.Identity.Abstractions.Helpers;
using GR.Identity.Clients.Abstractions.Extensions;
using GR.Identity.Clients.Infrastructure;
using GR.Identity.Clients.Infrastructure.Data;
using GR.Identity.Clients.Razor.Extensions;
using GR.Identity.Groups.Abstractions.Extensions;
using GR.Identity.Groups.Infrastructure;
using GR.Identity.Groups.Infrastructure.Data;
using GR.Identity.Permissions.Abstractions.Configurators;
using GR.Identity.PhoneVerification.Abstractions.Extensions;
using GR.Identity.PhoneVerification.Infrastructure;
using GR.Identity.Profile;
using GR.Identity.Profile.Abstractions.Extensions;
using GR.Identity.Profile.Data;
using GR.Identity.Razor.Extensions;
using GR.Localization.Abstractions.Models.Config;
using GR.Localization.Data;
using GR.Localization.JsonStringProvider;
using GR.Logger;
using GR.Logger.Abstractions.Extensions;
using GR.Procesess.Data;
using GR.Procesess.Parsers;
using GR.Process.Razor.Extensions;
using GR.Processes.Abstractions.Extensions;
using GR.Processes.Abstractions.Helpers;
using Microsoft.Extensions.Logging;
using ProfileService = GR.Identity.Clients.Infrastructure.ProfileService;

#endregion Usings

namespace GR.Cms
{
	public class Startup : GearCoreStartup
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="configuration"></param>
		/// <param name="env"></param>
		public Startup(IConfiguration configuration, IHostingEnvironment env) : base(configuration, env) { }

		/// <summary>
		/// Configure cms app
		/// </summary>
		/// <param name="app"></param>
		public override void Configure(IApplicationBuilder app)
		{
			app.UseGearWebApp(config =>
			{
				config.AppName = "Web APP";
				config.HostingEnvironment = HostingEnvironment;
				config.Configuration = Configuration;
			});

			app.UseIdentityServer();
			app.UseUrlRewriteModule();
		}

		/// <summary>
		/// This method gets called by the runtime. Use this method to add services to the container.
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		public override IServiceProvider ConfigureServices(IServiceCollection services) =>
			services.RegisterGearWebApp(config =>
		{
			config.Configuration = Configuration;
			config.HostingEnvironment = HostingEnvironment;
			config.CacheConfiguration.UseInMemoryCache = true;

			//------------------------------Identity Module-------------------------------------
			config.GearServices.AddIdentityModule<GearIdentityDbContext>()
				.RegisterModulePermissionConfigurator<DefaultPermissionsConfigurator<UserPermissions>, UserPermissions>()
				.RegisterModulePermissionConfigurator<DefaultPermissionsConfigurator<RolePermissions>, RolePermissions>()
				//.PasswordPolicy(options =>
				//{
				//	options.RequireDigit = true;
				//	options.RequiredLength = 6;
				//	options.RequireNonAlphanumeric = false;
				//	options.RequireUppercase = false;
				//	options.RequireLowercase = false;
				//})
				.AddIdentityUserManager<IdentityUserManager, GearUser>()
				.AddIdentityModuleStorage<GearIdentityDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				})
				.AddIdentityModuleEvents()
				.AddIdentityRazorModule();

			//-----------------------------Authentification Module-------------------------------------
			config.GearServices.AddAuthentication(Configuration);


			//-----------------------------Identity Clients Module-------------------------------------
			config.GearServices //DefaultClientsConfigurator
				.AddIdentityClientsModule<GearUser, ClientsConfigurationDbContext, ClientsPersistedGrantDbContext,
					ClientsConfigurator>(Configuration)
				.AddClientsProfileService<ProfileService>()
				.RegisterClientsService<ClientsService>()
				.AddApiClientsRazorModule();

			//---------------------------------------Groups Module-------------------------------------
			config.GearServices.AddUserGroupModule<GroupService, GearUser>()
				.AddUserGroupModuleStorage<GroupsDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				});

			//----------------------------------Permissions Module-------------------------------------
			config.GearServices.AddPermissionModule<PermissionService<GearIdentityDbContext>>()
				.MapPermissionsModuleToContext<GearIdentityDbContext>();

			//---------------------------------------Profile Module-------------------------------------
			config.GearServices.AddProfileModule<Identity.Profile.ProfileService>()
				.AddUserAddressService<UserAddressService>()
				.AddProfileModuleStorage<ProfileDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				});

			//---------------------------------------Entity Module-------------------------------------
			config.GearServices.AddEntityModule<EntitiesDbContext, EntityService>()
				.RegisterModulePermissionConfigurator<DefaultPermissionsConfigurator<EntityPermissions>, EntityPermissions>()
				.RegisterModulePermissionConfigurator<DefaultPermissionsConfigurator<EntityTypePermissions>, EntityTypePermissions>()
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
			config.GearServices.AddEntityRoleAccessModule<EntityRoleAccessService<EntitySecurityDbContext, GearIdentityDbContext>>()
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

			//-------------------------------Notification Module-------------------------------------
			config.GearServices.AddNotificationModule<NotifyWithDynamicEntities<GearIdentityDbContext, GearRole, GearUser>, GearRole>()
				.AddNotificationSubscriptionModule<NotificationSubscriptionService>()
				.AddNotificationModuleEvents()
				.AddNotificationSubscriptionModuleStorage<NotificationDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				})
				.AddNotificationRazorUIModule();

			//---------------------------------Localization Module-------------------------------------
			config.GearServices
				.AddLocalizationModule<JsonFileLocalizationService, JsonStringLocalizer>(
					Configuration.GetSection(nameof(LocalizationConfig)))
				.RegisterTranslationService<YandexTranslationProvider>(
					Configuration.GetSection(nameof(LocalizationProviderSettings)))
				//Only with DB provider
				//.AddLocalizationModuleStorage<TranslationsDbContext>(options =>
				//{
				//	options.GetDefaultOptions(Configuration);
				//	options.EnableSensitiveDataLogging();
				//})
				.AddLocalizationRazorModule()
				.AddCountryModule<CountryService>()
				.AddCountryModuleStorage<CountriesDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				});

			//--------------------------------------Menu UI Module-------------------------------------
			config.GearServices.AddMenuModule<MenuService>()
				.AddMenuModuleStorage<MenuDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				});

			//------------------------------Database backup Module-------------------------------------
			config.GearServices
				.RegisterDatabaseBackupRunnerModule<PostGreSqlBackupSettings, PostGreBackupService>(Configuration)
				.RegisterDatabaseBackgroundService<BackupTimeService<PostGreSqlBackupSettings>>()
				.AddBackupRazorModule();

			//------------------------------------Calendar Module------------------------------------ -
			config.GearServices
				.RegisterModulePermissionConfigurator<DefaultPermissionsConfigurator<CalendarPermissions>, CalendarPermissions>()
				.AddCalendarModule<CalendarManager>()
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

			//Register files razor provider 
			config.GearServices.AddFilesRazorModule();
			//------------------------------------Task Module-------------------------------------
			config.GearServices.AddTaskModule<TaskManager.Services.TaskManager, TaskManagerNotificationService>()
				.AddTaskModuleStorage<TaskManagerDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				})
				.AddTaskManagerRazorUIModule();

			//-----------------------------------------Forms Module-------------------------------------
			config.GearServices.AddFormModule<FormDbContext>()
				.RegisterModulePermissionConfigurator<DefaultPermissionsConfigurator<FormsPermissions>, FormsPermissions>()
				.AddFormModuleStorage<FormDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				})
				.RegisterFormService<FormService<FormDbContext>>()
				.AddFormStaticFilesModule();

			//-----------------------------------------Page Module-------------------------------------
			config.GearServices.AddPageModule()
				.AddPageModuleStorage<DynamicPagesDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				})
				.AddPageRenderUIModule<PageRender.PageRender>()
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

			//----------------------------------------Installer Module-------------------------------------
			config.GearServices.AddInstallerModule<GearWebInstallerService>();

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
				//Paypal
				.RegisterPaypalProvider<PaypalPaymentMethodService>()
				.RegisterPaypalRazorProvider(Configuration)
				//Mobil Pay
				.RegisterMobilPayProvider<MobilPayPaymentMethodService>()
				.RegisterMobilPayRazorProvider(Configuration)
				//Braintree
				.RegisterBraintreeProvider<BraintreePaymentMethodService>()
				.RegisterBraintreeRazorProvider(Configuration)
				.AddCommerceRazorUIModule();

			//---------------------------------Multi Tenant Module-------------------------------------
			config.GearServices.AddTenantModule<OrganizationService, Tenant>()
				.AddMultiTenantRazorUIModule();

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
				.RegisterDocumentServices<DocumentWithWorkflowService>()
				.AddDocumentRazorUIModule();


			//------------------------------------ Logging Module -----------------------------------
			config.GearServices.RegisterLoggerModule<GearLoggerFactory>()
				.AddLoggingConfiguration(logging =>
				{
					logging.SetMinimumLevel(LogLevel.Trace);
					logging.AddSeq(Configuration.GetSection("Logging:Seq"));
					//logging.ClearProviders();
					//logging.AddFilter("Microsoft", Microsoft.Extensions.Logging.LogLevel.Trace);
					//logging.AddFilter("System", Microsoft.Extensions.Logging.LogLevel.Trace);
				});


			//-------------------------- Phone verification Module ----------------------------------
			config.GearServices.AddPhoneVerificationModule<Authy>();

			//------------------------------------Processes Module-------------------------------------
			config.GearServices.AddProcessesModule<ProcessParser>()
				.RegisterModulePermissionConfigurator<DefaultPermissionsConfigurator<ProcessesPermissions>, ProcessesPermissions>()
				.AddProcessesModuleStorage<ProcessesDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				})
				.AddProcessesRazorModule();
		});
	}
}