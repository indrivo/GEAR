#region Usings

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GR.Backup.Abstractions.BackgroundServices;
using GR.Backup.Abstractions.Extensions;
using GR.Backup.PostGresSql;
using GR.Core.Extensions;
using GR.DynamicEntityStorage.Extensions;
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
using GR.Application.Middleware.Extensions;
using GR.Audit;
using GR.Audit.Abstractions.Extensions;
using GR.Dashboard;
using GR.Dashboard.Abstractions;
using GR.Dashboard.Abstractions.Extensions;
using GR.Dashboard.Abstractions.Models.WidgetTypes;
using GR.Dashboard.Data;
using GR.Dashboard.Razor.Extensions;
using GR.Dashboard.Renders;
using GR.Email.Razor.Extensions;
using GR.Entities.Security.Razor.Extensions;
using GR.MultiTenant.Abstractions.Extensions;
using GR.MultiTenant.Razor.Extensions;
using GR.MultiTenant.Services;
using GR.Report.Dynamic.Razor.Extensions;
using GR.DynamicEntityStorage.Abstractions;
using GR.Entities.Extensions;
using GR.Localization;
using GR.PageRender;
using GR.Identity.IdentityServer4.Extensions;
using GR.Identity.LdapAuth.Abstractions.Models;
using GR.Identity.Permissions.Abstractions.Extensions;
using GR.Report.Dynamic;
using GR.Report.Dynamic.Data;
using GR.WebApplication.Extensions;

#endregion

namespace GR.Demo
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

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// App configuration
        /// </summary>
        /// <param name="app"></param>
        public void Configure(IApplicationBuilder app) => app.UseGearWebApp(config =>
        {
            config.HostingEnvironment = HostingEnvironment;
            config.Configuration = Configuration;
        });

        /// <summary>
        /// Service configuration
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
            => services.RegisterGearWebApp(config =>
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

				//---------------------------------Multi Tenant Module-------------------------------------
				config.GearServices.AddTenantModule<OrganizationService, Tenant>()
					.AddMultiTenantRazorUIModule();
			});
    }
}
