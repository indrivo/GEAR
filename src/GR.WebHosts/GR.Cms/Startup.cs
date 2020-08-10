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
using GR.ECommerce.Abstractions.Extensions;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Payments.Abstractions.Extensions;
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
using GR.Notifications.Abstractions.Extensions;
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
using System.Threading.Tasks;
using GR.AccountActivity.Abstractions.ActionFilters;
using GR.AccountActivity.Abstractions.Extensions;
using GR.AccountActivity.Impl;
using GR.AccountActivity.Impl.Data;
using GR.ApplePay;
using GR.ApplePay.Abstractions.Extensions;
using GR.ApplePay.Razor.Extensions;
using GR.Backup.Razor.Extensions;
using GR.Braintree;
using GR.Braintree.Abstractions.Extensions;
using GR.Braintree.Razor.Extensions;
using GR.Calendar.Abstractions.Helpers;
using GR.Card.Abstractions.Extensions;
using GR.Card.AuthorizeDotNet;
using GR.Card.Razor.Extensions;
using GR.Core;
using GR.Core.UI.Razor.DefaultTheme.Extensions;
using GR.Forms;
using GR.Localization.Razor.Extensions;
using GR.UI.Menu;
using GR.UI.Menu.Abstractions.Extensions;
using GR.UI.Menu.Data;
using GR.Documents.Razor.Extensions;
using GR.DynamicEntityStorage.Extensions;
using GR.ECommerce.Abstractions.Helpers.PermissionConfigurations;
using GR.ECommerce.Infrastructure.Data;
using GR.ECommerce.Infrastructure.Services;
using GR.EmailTwoFactorAuth;
using GR.EmailTwoFactorAuth.Extensions;
using GR.Entities.Abstractions;
using GR.Entities.Abstractions.Helpers;
using GR.Files.Razor.Extensions;
using GR.Forms.Abstractions.Helpers;
using GR.GooglePay;
using GR.GooglePay.Abstractions.Extensions;
using GR.GooglePay.Razor.Extensions;
using GR.Identity;
using GR.Identity.Abstractions.Helpers;
using GR.Identity.Abstractions.Helpers.PasswordPolicies;
using GR.Identity.Clients.Abstractions.Extensions;
using GR.Identity.Clients.Infrastructure;
using GR.Identity.Clients.Infrastructure.Data;
using GR.Identity.Clients.Razor.Extensions;
using GR.Identity.Groups.Abstractions.Extensions;
using GR.Identity.Groups.Infrastructure;
using GR.Identity.Groups.Infrastructure.Data;
using GR.Identity.Mpass;
using GR.Identity.Mpass.Abstractions.Extensions;
using GR.Identity.Mpass.Abstractions.Helpers;
using GR.Identity.Mpass.Abstractions.Security;
using GR.Identity.Permissions.Abstractions.Configurators;
using GR.Identity.Permissions.Api.Helpers;
using GR.Identity.PhoneVerification.Abstractions.Extensions;
using GR.Identity.PhoneVerification.Infrastructure;
using GR.Identity.Profile;
using GR.Identity.Profile.Abstractions.Extensions;
using GR.Identity.Profile.Data;
using GR.Identity.Razor.Extensions;
using GR.Localization.Abstractions.Models.Config;
using GR.Localization.Api.Helpers;
using GR.Localization.Data;
using GR.Localization.Extensions;
using GR.Localization.JsonStringProvider;
using GR.Logger;
using GR.Logger.Abstractions.Extensions;
using GR.Notifications.Hub.Hubs;
using GR.Procesess.Data;
using GR.Procesess.Parsers;
using GR.Process.Razor.Extensions;
using GR.Processes.Abstractions.Extensions;
using GR.Processes.Abstractions.Helpers;
using GR.UserPreferences.Abstractions.Extensions;
using GR.UserPreferences.Abstractions.Helpers;
using GR.UserPreferences.Impl;
using GR.UserPreferences.Impl.Data;
using Microsoft.Extensions.Logging;
using ProfileService = GR.Identity.Clients.Infrastructure.ProfileService;
using GR.Localization.ExternalProviders;
using GR.Notifications.Dynamic;
using GR.Notifications.Dynamic.Seeders;
using GR.Notifications.Subscriptions.Abstractions.Extensions;
using GR.Notifications.Subscriptions.EFCore;
using GR.Notifications.Subscriptions.EFCore.Data;
using GR.Notifications.Subscriptions.Razor.Extensions;
using GR.TwoFactorAuthentication.Abstractions.Extensions;
using GR.WebApplication.Helpers.AppConfigurations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using Serilog;

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
		public Startup(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

		/// <summary>
		/// Configure cms app
		/// </summary>
		/// <param name="app"></param>
		public override void Configure(IApplicationBuilder app)
		{
			app.UseUrlRewriteModule();
			app.UseGearWebApp(config =>
			{
				config.AppName = "Gear";
				config.HostingEnvironment = HostingEnvironment;
				config.UseHealthCheck = false;
				config.Configuration = Configuration;
				config.MvcTemplate = "{controller=Home}/{action=Index}";
				config.CustomMapRules = new Dictionary<string, Action<HttpContext>>
				{
					{ "/admin", context => context.MapTo("/Account/Login") }
				};
				config.AutoApplyPendingMigrations = true;
			});

			app.UseIdentityServer();
			app.UseNotificationsHub<GearNotificationHub>();
		}

		/// <summary>
		/// This method gets called by the runtime. Use this method to add services to the container.
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		public override void ConfigureServices(IServiceCollection services) =>
			services.RegisterGearWebApp(Configuration, HostingEnvironment, config =>
		{
			//------------------------------Global Config----------------------------------------
			config.CacheConfiguration.UseInMemoryCache = true;
			config.UseHotReload = true;
			config.UseHealthCheck = false;
			config.SwaggerServicesConfiguration = new SwaggerServicesConfiguration
			{
				UseSwagger = true,
				AuthenticationOperationFilterConfiguration = options =>
				{
					options.OpenApiOperations.Add(PermissionsOpenApiHelper.PermissionDocs);
					options.OpenApiOperations.Add(LocalizationOpenApiHelper.LocalizationDocs);
				}
			};

			//------------------------------Theme Config------------------------------------------
			config.GearServices.RegisterDefaultThemeRazorModule();

			//------------------------------Identity Module----------------------------------------
			config.GearServices.AddIdentityModule<GearIdentityDbContext>(o =>
				{
					o.Lockout = new LockoutOptions
					{
						MaxFailedAccessAttempts = 5,
						AllowedForNewUsers = true,
						DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2)
					};
				})
				.AddConfirmDeviceTokenProvider();

			config.GearServices.RegisterModulePermissionConfigurator<DefaultPermissionsConfigurator<UserPermissions>, UserPermissions>()
				.RegisterModulePermissionConfigurator<DefaultPermissionsConfigurator<RolePermissions>, RolePermissions>()
				.PasswordPolicy(new DefaultPasswordPolicy())
				.AddIdentityUserManager<IdentityUserManager, GearUser>()
				.AddIdentityModuleStorage<GearIdentityDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				})
				.AddIdentityModuleEvents()
				.AddIdentityRazorModule();

			//-----------------------------Authentication Module-------------------------------------
			config.GearServices.AddAuthentication<AuthorizeService>()
				.RegisterTwoFactorAuthenticatorProvider<EmailTwoFactorAuthService>()
                .EmailTwoFactorAuthConfiguration(options =>
				{
					options.UseHtmlTemplate = false;
				});

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
				.AddEntityRazorUiModule();

			//------------------------------Entity Security Module-------------------------------------
			config.GearServices.AddEntityRoleAccessModule<EntityRoleAccessService<EntitySecurityDbContext, GearIdentityDbContext>>()
				.AddEntityModuleSecurityStorage<EntitySecurityDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				})
				.AddEntitySecurityRazorUiModule();

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
				.AddDashboardRazorUiModule()
				.AddDashboardRenderServices(new Dictionary<Type, Type>
				{
					{typeof(IWidgetRenderer<ReportWidget>), typeof(ReportWidgetRender)},
					{typeof(IWidgetRenderer<CustomWidget>), typeof(CustomWidgetRender)},
				})
				.RegisterProgramAssembly(typeof(Program));

			//-------------------------------Dynamic Notification Module-------------------------------------
			config.GearServices.AddNotificationModule<NotifyWithDynamicEntities<GearIdentityDbContext, GearRole, GearUser>, GearRole>()
				.AddNotificationSeeder<DynamicNotificationSeederService>()
				.RegisterNotificationsHubModule<CommunicationHub>()
				.AddNotificationRazorUIModule();

			////-------------------------------EF core Notification Module-------------------------------------
			//config.GearServices.AddNotificationModule<Notify<GearIdentityDbContext, GearRole, GearUser>, GearRole>()
			//	.AddNotificationSeeder<EfCoreNotificationSeederService>()
			//	.AddNotificationModuleStorage<NotificationDbContext>(options =>
			//	{
			//		options.GetDefaultOptions(Configuration);
			//		options.EnableSensitiveDataLogging();
			//	})
			//	.RegisterNotificationsHubModule<CommunicationHub>()
			//	.AddNotificationRazorUIModule();

			//-------------------------------Notification subscriptions Module-------------------------------------
			config.GearServices.AddNotificationSubscriptionModule<NotificationSubscriptionService>()
				.AddNotificationModuleEvents()
				.AddNotificationSubscriptionModuleStorage<NotificationsSubscriptionDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				})
				.AddNotificationSubscriptionRazorUiModule();

			//---------------------------------Localization Module-------------------------------------
			config.GearServices
				.AddLocalizationModule<JsonFileLocalizationService, JsonStringLocalizer>()
				.BindLanguagesFromJsonFile(Configuration.GetSection(nameof(LocalizationConfig)))
				.RegisterTranslationService<YandexTranslationProvider>(
					Configuration.GetSection(nameof(LocalizationProviderSettings)))
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
				.AddCalendarRazorUiModule()
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
				.AddTaskManagerRazorUiModule();

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
				.AddReportUiModule();

			//----------------------------------------Installer Module-------------------------------------
			config.GearServices.AddInstallerModule<GearWebInstallerService>();

			//----------------------------------------Email Module-------------------------------------
			config.GearServices.AddEmailModule<EmailSender>()
				.AddEmailRazorUIModule()
				.BindEmailSettings(Configuration);

			//----------------------------------------Ldap Module-------------------------------------
			config.GearServices
				.AddIdentityLdapModule<LdapUser, LdapService<LdapUser>, LdapUserManager<LdapUser>>(Configuration)
				.AddLdapAuthentication<LdapAuthorizeService>(options =>
				{
					options.AutoImportOnLogin = true;
				});

			//-------------------------------------------MPass Module-------------------------------------
			config.GearServices.AddMPassModuleSigningCredentials(new MPassSigningCredentials
			{
				ServiceProviderCertificate = MPassResources.GetSandboxServiceProviderCertificate(),
				IdentityProviderCertificate = MPassResources.GetSandboxIdentityProviderCertificate()
			})
				.AddMPassService<MPassService>();

			//-------------------------------------Commerce module-------------------------------------
			config.GearServices.RegisterCommerceModule<CommerceDbContext>()
				.RegisterCommerceProductRepository<ProductService, Product>()
				.RegisterCommerceStorage<CommerceDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				})
				.RegisterCartService<CartService>()
				.RegisterBrandService<BrandService>()
				.RegisterProductAttributeService<ProductAttributeService>()
				.RegisterProductTypeService<ProductTypeService>()
				.RegisterProductCategoryService<ProductCategoryService>()
				.RegisterModulePermissionConfigurator<DefaultPermissionsConfigurator<BrandPermissions>, BrandPermissions>()
				.RegisterCommerceEvents()
				.AddCommerceRazorUiModule();

			//-----------------------------------Commerce payments module------------------------------
			config.GearServices.RegisterPayments<PaymentService>()
				.RegisterPaymentStorage<CommerceDbContext>()
				//Paypal
				.RegisterPaypalProvider<PaypalPaymentMethodService>()
				.RegisterPaypalRazorProvider(Configuration)
				//Braintree
				.RegisterBraintreeProvider<BraintreePaymentMethodService>()
				.RegisterBraintreeRazorProvider(Configuration)
				//GPay
				.RegisterGPayProvider<GPayPaymentMethodService>()
				.RegisterGPayRazorProvider(Configuration)
				//ApplePay
				.RegisterApplePayProvider<ApplePayPaymentMethodService>()
				.RegisterApplePayRazorProvider(Configuration)
				//CreditCard
				.RegisterCreditCardPayProvider<AuthorizeDotNetPaymentMethodService>(options =>
				{
					options.VerificationCardCurrencyCode = "USD";
					options.VerificationCardValue = 0.01M;
				})
				.RegisterCreditCardRazorProvider(Configuration)
				//Mobil Pay
				.RegisterMobilPayProvider<MobilPayPaymentMethodService>()
				.RegisterMobilPayRazorProvider(Configuration);

			//-------------------------------------Commerce orders module------------------------------
			config.GearServices.RegisterProductOrderServices<Order, OrderProductService>()
				.RegisterOrdersStorage<CommerceDbContext>()
				.RegisterOrderEvents();

			//-------------------------------------Subscription module---------------------------------
			config.GearServices.RegisterSubscriptionServices<Subscription, SubscriptionService>(options =>
				{
					options.NotificationProviders = new List<string>
					{
						"email", "notification.local"
					};
				})
				.RegisterBackgroundService<SubscriptionValidationBackgroundService>()
				.RegisterSubscriptionEvents()
				.RegisterSubscriptionRules()
				.RegisterSubscriptionStorage<CommerceDbContext>();

			//---------------------------------Multi Tenant Module-------------------------------------
			config.GearServices.AddTenantModule<OrganizationService, Tenant>()
				.AddMultiTenantRazorUiModule();

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
				.AddDocumentRazorUiModule();


			//------------------------------------ Logging Module -----------------------------------
			config.GearServices.RegisterLoggerModule<GearLoggerFactory>()
				.AddLoggingConfiguration(logging =>
				{
					logging.SetMinimumLevel(LogLevel.Trace);
					logging.AddSerilog(new LoggerConfiguration()
						.ReadFrom.Configuration(Configuration)
						.Destructure.AsScalar<JObject>()
						.Destructure.AsScalar<JArray>()
						.Enrich.WithProperty("ApplicationName", "Gear")
						.Enrich.WithProperty("ApplicationVersion", GearApplication.AppVersion)
						.CreateLogger(), true);
					logging.AddSentry();
				});


			//-------------------------- Phone verification Module ----------------------------------
			config.GearServices.AddPhoneVerificationModule<Authy>(Configuration)
				.AddSmsSenderModule<SmsSenderService>()
				.BindPhoneVerificationEvents();

			//---------------------------------User activity  Module ----------------------------------
			services.AddUserActivityModule<UserActivityService, ActivityTrackerFilter>()
				.AddUserActivityModuleStorage<UserActivityDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				});


			//---------------------------------User preferences Module --------------------------------
			config.GearServices
				.AddUserPreferencesModule<UserPreferencesService>()
				.RegisterPreferencesProvider<DefaultUserPreferenceProvider>()
				.AddUserPreferencesModuleStorage<UserPreferencesDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				});


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

		/// <summary>
		/// Run before start migrations
		/// </summary>
		/// <param name="webHost"></param>
		/// <returns></returns>
		public override Task OnBeforeDatabaseMigrationsApply(IHost webHost)
		{
			webHost.MigrateAbstractDbContext<IEntityContext>()
				.MigrateAbstractDbContext<IIdentityContext>();

			return base.OnBeforeDatabaseMigrationsApply(webHost);
		}
	}
}