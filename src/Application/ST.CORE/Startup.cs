using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ST.CORE.Extensions;
using ST.CORE.Installation;
using ST.CORE.LoggerTargets;
using ST.CORE.Services;
using ST.CORE.Services.Abstraction;
using ST.Entities.Data;
using ST.Entities.Extensions;
using ST.Identity.Abstractions;
using ST.Identity.Data.Permissions;
using ST.Identity.Data.UserProfiles;
using ST.Identity.Extensions;
using ST.Identity.LDAP.Models;
using ST.Identity.Services;
using ST.Identity.Versioning;
using ST.Localization;
using ST.MPass.Gov;
using ST.Identity.Data;
using ST.Notifications.Extensions;
using ST.Procesess.Data;

namespace ST.CORE
{
	public class Startup
	{
		/// <summary>
		/// Cookie name
		/// </summary>
		private const string CookieName = ".ST.ISO.Data";

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
			app.UseSignalR();
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			var migrationsAssembly = typeof(Identity.DbSchemaNameConstants).GetTypeInfo().Assembly.GetName().Name;

			services.Configure<SecurityStampValidatorOptions>(options =>
			{
				// enables immediate logout, after updating the user's stat.
				options.ValidationInterval = TimeSpan.Zero;
			});
			services.AddConfiguredCors();

			services.AddLocalization(Configuration);
			services.AddDbContext<EntitiesDbContext>(options =>
			{
				options.GetDefaultOptions(Configuration, HostingEnvironment);
			});
			services.AddDbContext<ProcessesDbContext>(options =>
			{
				options.GetDefaultOptions(Configuration, HostingEnvironment);
			});

			services.AddDbContextAndIdentity(Configuration, HostingEnvironment, migrationsAssembly, HostingEnvironment)
				.AddApplicationSpecificServices(HostingEnvironment)
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
					opts.Cookie.Name = CookieName;
				})
				.AddAuthenticationAndAuthorization(HostingEnvironment, Configuration)
				.AddIdentityServer(Configuration, HostingEnvironment, migrationsAssembly)
				.AddHealthChecks(checks =>
				{
					//var minutes = 1;
					//if (int.TryParse(Configuration["HealthCheck:Timeout"], out var minutesParsed))
					//	minutes = minutesParsed;

					//checks.AddSqlCheck("ApplicationDbContext-DB", connectionString.Item2, TimeSpan.FromMinutes(minutes));
				});

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
			//Add signaler
			services.AddSignalR<ApplicationDbContext, ApplicationUser, ApplicationRole>();

			//Run background service
			services.AddHostedService<HostedTimeService>();

			services.AddScoped<ILocalService, LocalService>();

			services.AddTransient<ITreeIsoService, TreeIsoService>();

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
				var env = serviceScope.ServiceProvider.GetService<IHostingEnvironment>();
				var isConfigured = Application.IsConfigured(env);

				if (isConfigured)
				{
					var permissionService = serviceScope.ServiceProvider.GetService<IPermissionService>();
					await permissionService.RefreshCache();
				}
			}
		}
	}
}