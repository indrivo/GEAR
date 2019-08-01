using System;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ST.Application;
using ST.Backup.Abstractions.BackgroundServices;
using ST.Backup.Abstractions.Extensions;
using ST.Backup.PostGresSql;
using ST.Cache.Extensions;
using ST.Configuration.Extensions;
using ST.Configuration.Server;
using ST.DynamicEntityStorage.Extensions;
using ST.Entities.Data;
using ST.Entities.Extensions;
using ST.Identity.Abstractions;
using ST.Identity.Data;
using ST.Identity.Versioning;
using ST.Localization;
using ST.MPass.Gov;
using ST.Notifications.Extensions;
using ST.PageRender.Razor.Extensions;
using ST.Procesess.Data;
using ST.Process.Razor.Extensions;
using ST.Cms.Services.Abstractions;
using ST.Core;
using ST.Core.Extensions;
using ST.Core.Helpers.DbContexts;
using ST.Entities;
using ST.Identity.Models.EmailViewModels;
using ST.InternalCalendar.Razor.Extensions;
using ST.Report.Dynamic.Data;
using ST.Report.Dynamic.Extensions;
using ST.Entities.Abstractions.Extensions;
using ST.Entities.EntityBuilder.Postgres;
using ST.Entities.EntityBuilder.Postgres.Controls.Query;
using ST.Forms.Abstractions.Extensions;
using ST.Forms.Data;
using ST.Forms.Razor.Extensions;
using ST.Identity.LdapAuth;
using ST.Identity.LdapAuth.Abstractions.Extensions;
using ST.Install;
using ST.Install.Abstractions.Extensions;
using ST.Localization.Abstractions;
using ST.Localization.Abstractions.Extensions;
using ST.Localization.Abstractions.Models;
using ST.Localization.Services;
using ST.PageRender.Abstractions.Extensions;
using ST.PageRender.Data;
using TreeIsoService = ST.Cms.Services.TreeIsoService;

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
			lifetime.ApplicationStarted.Register(() =>
			{
				CoreApp.OnApplicationStarted(app);
			});

			lifetime.RegisterAppEvents(nameof(MigrationsAssembly));

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

			//--------------------------------------Cors origin Module-------------------------------------
			services.AddOriginCorsModule();

			services.AddDbContext<ProcessesDbContext>(options =>
			{
				options.GetDefaultOptions(Configuration, HostingEnvironment);
				options.EnableSensitiveDataLogging();
			});

			//------------------------------Identity Module-------------------------------------
			services.AddIdentityModule(Configuration, HostingEnvironment, MigrationsAssembly, HostingEnvironment)
				.AddApplicationSpecificServices(HostingEnvironment, Configuration)
				.AddDistributedMemoryCache()
				.AddMvc()
				.AddJsonOptions(x =>
				{
					x.SerializerSettings.DateFormatString = Settings.Date.DateFormat;
				});

			services.AddAuthenticationAndAuthorization(HostingEnvironment, Configuration)
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
			services.AddEntityModule<EntitiesDbContext, EntityRepository, NpgTableQueryBuilder, NpgEntityQueryBuilder, NpgTablesService>();
			services.AddDbContext<EntitiesDbContext>(options =>
			{
				options.GetDefaultOptions(Configuration, HostingEnvironment);
				options.EnableSensitiveDataLogging();
			}, ServiceLifetime.Transient);

			//---------------------------Dynamic repository Module-------------------------------------
			services.AddDynamicDataProviderModule<EntitiesDbContext>();

			//--------------------------------------SignalR Module-------------------------------------
			services.AddSignalRModule<ApplicationDbContext, ApplicationUser, ApplicationRole>();

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
			services.RegisterDatabaseBackupRunnerModule<BackupTimeService<PostGreSqlBackupSettings>, PostGreSqlBackupSettings, PostGreBackupService>(Configuration);

			//------------------------------------Page render Module-------------------------------------
			services.AddPageRenderUiModule();

			//------------------------------------Processes Module-------------------------------------
			services.AddProcessesModule();

			//----------------------------Internal calendar Module-------------------------------------
			services.AddInternalCalendarModule();

			//-----------------------------------------Form Module-------------------------------------
			services.AddFormModule<FormDbContext>();
			services.AddDbContext<FormDbContext>(options =>
			{
				options.GetDefaultOptions(Configuration, HostingEnvironment);
				options.EnableSensitiveDataLogging();
			});

			services.AddFormStaticFilesModule();


			//-----------------------------------------Page Module-------------------------------------
			services.AddPageModule<DynamicPagesDbContext>();
			services.AddDbContext<DynamicPagesDbContext>(options =>
			{
				options.GetDefaultOptions(Configuration, HostingEnvironment);
				options.EnableSensitiveDataLogging();
				var factoryOptions = new DbContextOptionsBuilder<EntitiesDbContext>();
				factoryOptions.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=1111;Database=ISODMS.DEV;");
				DbContextFactory<DynamicPagesDbContext, EntitiesDbContext>.Options = factoryOptions;
			});

			//---------------------------------------Report Module-------------------------------------
			services.AddDynamicReportModule<DynamicReportDbContext>();
			services.AddDbContext<DynamicReportDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration, HostingEnvironment);
					options.EnableSensitiveDataLogging();
				});

			//---------------------------------Custom cache Module-------------------------------------
			services.UseCustomCacheModule(HostingEnvironment, Configuration);

			services.AddInstallerModule<SyncInstaller>();

			//----------------------------------------Email Module-------------------------------------
			services.Configure<EmailSettingsViewModel>(Configuration.GetSection("EmailSettings"));


			if (CoreApp.IsHostedOnLinux())
			{
				services.Configure<ForwardedHeadersOptions>(options =>
				{
					options.KnownProxies.Add(IPAddress.Parse("185.131.222.95"));
				});
			}

			//----------------------------------------Ldap Module-------------------------------------
			services.AddIdentityLdapModule<ApplicationUser, LdapService<ApplicationUser>, LdapUserManager<ApplicationUser>>(Configuration);


			//------------------------------------------Custom ISO-------------------------------------
			services.AddTransient<ITreeIsoService, TreeIsoService>();

			//--------------------------Custom dependency injection-------------------------------------
			return services.AddWindsorContainers();
		}
	}
}