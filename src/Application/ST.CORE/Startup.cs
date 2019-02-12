using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Core.Versioning;
using ST.CORE.Extensions;
using ST.CORE.Extensions.Installer;
using ST.Entities.Data;
using ST.Entities.Extensions;
using ST.Entities.Services.Abstraction;
using ST.Entities.Utils;
using ST.Identity.Abstractions;
using ST.Identity.Extensions;
using ST.Identity.LDAP.Models;
using ST.Identity.Services;
using ST.Localization;
using ST.MPass.Gov;
using ST.Notifications.Extensions;
using ST.Procesess.Data;

namespace ST.CORE
{
	public class Startup
	{
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
		/// AppSettings configuration
		/// </summary>
		private IConfiguration Configuration { get; }

		/// <summary>
		/// Hosting configuration
		/// </summary>
		private IHostingEnvironment HostingEnvironment { get; }

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
			IOptionsSnapshot<LocalizationConfig> languages, IApplicationLifetime lifetime)
		{
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

			app.UsePageRedirect();

			app.UseConfiguredCors(Configuration);

			app.UseSwagger()
				.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "ST.CORE API v1.0"); });

			app.UseLocalization(languages);
			lifetime.ApplicationStarted.Register(() =>
			{
				OnApplicationStarted(app);
			});
			// Microsoft.AspNetCore.StaticFiles: API for starting the application from wwwroot.
			// Uses default files as index.html.
			app.UseDefaultFiles();
			app.UseStaticFiles();
			app.UseStSignalR();
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			var connectionString = ConnectionString.Get(Configuration, HostingEnvironment);

			var migrationsAssembly = typeof(Identity.Constants).GetTypeInfo().Assembly.GetName().Name;

			services.Configure<SecurityStampValidatorOptions>(options =>
			{
				// enables immediate logout, after updating the user's stat.
				options.ValidationInterval = TimeSpan.Zero;
			});
			services.AddConfiguredCors();

			services.AddStLocalization(Configuration);
			services.AddDbContext<EntitiesDbContext>(options =>
			{
				options = options.GetDefaultOptions(connectionString);
			});
			services.AddDbContext<ProcessesDbContext>(options =>
			{
				options = options.GetDefaultOptions(connectionString);
			});

			services.AddDbContextAndIdentity(connectionString, migrationsAssembly, HostingEnvironment)
				.AddApplicationSpecificServices()
				.AddMPassSigningCredentials(new MPassSigningCredentials
				{
					ServiceProviderCertificate =
						new X509Certificate2("Certificates/samplempass.pfx", "qN6n31IT86684JO"),
					IdentityProviderCertificate = new X509Certificate2("Certificates/testmpass.cer")
				})
				.AddMvc();

			services.AddDistributedMemoryCache()
				.AddSession(opts =>
				{
					opts.Cookie.HttpOnly = true;
					opts.Cookie.Name = ".ST.CORE.Data";
				})
				.AddAuthenticationAndAuthorization(HostingEnvironment, Configuration)
				.AddIdentityServer(connectionString, migrationsAssembly)
				.AddHealthChecks(checks =>
				{
					var minutes = 1;
					if (int.TryParse(Configuration["HealthCheck:Timeout"], out var minutesParsed))
						minutes = minutesParsed;

					checks.AddSqlCheck("ApplicationDbContext-DB", connectionString.Item2, TimeSpan.FromMinutes(minutes));
				});

			services.AddAdditionalAuthetificationProviders(Configuration);

			services.AddApiVersioning(options =>
			{
				options.ReportApiVersions = true;
				options.AssumeDefaultVersionWhenUnspecified = true;
				options.DefaultApiVersion = new ApiVersion(1, 0);
				options.ErrorResponses = new UnsupportedApiVersionErrorResponseProvider();
			});
			services.Configure<LdapSettings>(Configuration.GetSection(nameof(LdapSettings)));
			//Add configured swagger
			services.AddSwagger(Configuration, HostingEnvironment);
			//Repository
			services.AddBussinessRepository();

			//Register dynamic table repository
			services.RegisterDynamicDataServices();
			//Add signalar
			services.AddStSignalR();

			//Run background service
			//services.AddHostedService<HostedTimeService>();

			services.AddScoped<ILocalService, LocalService>();
			//Register dependencies
			return services.AddWindsorContainers();
		}
		/// <summary>
		/// On application start
		/// </summary>
		/// <param name="app"></param>
		private static async void OnApplicationStarted(IApplicationBuilder app)
		{
			using (var serviceScope = app.ApplicationServices
			   .GetRequiredService<IServiceScopeFactory>()
			   .CreateScope())
			{
				var dataService = serviceScope.ServiceProvider.GetService<IDynamicEntityDataService>();
				var permissionService = serviceScope.ServiceProvider.GetService<IPermissionService>();
				await permissionService.RefreshCache();
				await EntitiesDbContextSeed.SeedNotificationTypesAsync(dataService);

				//Sync default menus
				await MenuSyncExtension.SyncMenuItems(dataService);
			}
		}
	}
}