using System;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ST.Backup.Extensions;
using ST.Configuration.Extensions;
using ST.Configuration.Server;
using ST.DynamicEntityStorage.Extensions;
using ST.Entities.Data;
using ST.Entities.Extensions;
using ST.Identity.Abstractions;
using ST.Identity.Abstractions.Ldap.Models;
using ST.Identity.Data;
using ST.Identity.Services;
using ST.Identity.Versioning;
using ST.Localization;
using ST.Localization.Razor.Extensions;
using ST.MPass.Gov;
using ST.Notifications.Extensions;
using ST.PageRender.Razor.Extensions;
using ST.Procesess.Data;
using ST.Process.Razor.Extensions;
using ST.WebHost.Services.Abstractions;
using TreeIsoService = ST.WebHost.Services.TreeIsoService;

namespace ST.WebHost
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
			if (Installation.Application.IsHostedOnLinux())
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

			app.UseUrlRewrite();

			app.UseConfiguredCors(Configuration);

			app.UseSwagger()
				.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "ST.CORE API v1.0"); });

			app.UseLocalization(languages);
			lifetime.ApplicationStarted.Register(() =>
			{
				Installation.Application.OnApplicationStarted(app);
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
				.AddApplicationSpecificServices(HostingEnvironment, CookieName)
				.AddMPassSigningCredentials(new MPassSigningCredentials
				{
					ServiceProviderCertificate =
						new X509Certificate2("Certificates/samplempass.pfx", "qN6n31IT86684JO"),
					IdentityProviderCertificate = new X509Certificate2("Certificates/testmpass.cer")
				})
				.AddDistributedMemoryCache()
				.AddMvc()
				.AddJsonOptions(x =>
				{
					x.SerializerSettings.DateFormatString = "dd'.'MM'.'yyyy hh:mm";
				});

			services.AddAuthenticationAndAuthorization(HostingEnvironment, Configuration)
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

			//Register dynamic table repository
			services.RegisterDynamicDataServices<EntitiesDbContext>();
			//Add signaler
			services.AddSignalR<ApplicationDbContext, ApplicationUser, ApplicationRole>();

			//Run background service
			//services.AddHostedService<HostedTimeService>();

			services.RegisterBackupRunner(Configuration);

			services.AddScoped<ILocalService, LocalService>();
			services.AddPageRender();
			services.AddProcesses();
			services.AddTransient<ITreeIsoService, TreeIsoService>();


			if (Installation.Application.IsHostedOnLinux())
			{
				services.Configure<ForwardedHeadersOptions>(options =>
				{
					options.KnownProxies.Add(IPAddress.Parse("185.131.222.95"));
				});
			}

			//Register dependencies
			return services.AddWindsorContainers();
		}
	}
}