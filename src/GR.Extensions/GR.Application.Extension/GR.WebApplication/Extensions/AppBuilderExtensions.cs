using System;
using System.IO;
using System.Linq;
using GR.Core;
using GR.Core.Extensions;
using GR.Core.Helpers.DbContexts;
using GR.Core.Razor.Extensions;
using GR.Localization.Abstractions.Extensions;
using GR.Localization.Abstractions.Models.Config;
using GR.WebApplication.Helpers;
using GR.WebApplication.Helpers.AppConfigurations;
using GR.WebApplication.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace GR.WebApplication.Extensions
{
    public static class AppBuilderExtensions
    {
        /// <summary>
        /// Use gear app
        /// </summary>
        /// <param name="app"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IGearAppBuilder UseGearWebApp(this IApplicationBuilder app, Action<GearAppBuilderConfig> config)
        {
            var configuration = new GearAppBuilderConfig();
            config(configuration);
            GearApplication.SetAppName(configuration.AppName);

            //---------------------------------------Heath Check---------------------------------------

            if (configuration.UseHealthCheck)
            {
                app.UseHealthChecks("/hc", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = GearHealthCheckWriter.WriteHealthCheckUiResponse
                });

                app.UseHealthChecksUI(o =>
                {
                    o.UIPath = "/gear-healthchecks-ui";
                });
            }
            var isConfigured = configuration.Configuration.GetValue<bool>("IsConfigured");
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Developed-By", "Indrivo");
                context.Response.Headers.Add("X-Powered-By-Framework", "Gear");
                context.Response.Headers.Add("X-App-Name", GearApplication.ApplicationName);
                context.Response.Headers.Add("X-App-Version", GearApplication.AppVersion);
                if (configuration.CustomMapRules == null || !isConfigured)
                {
                    await next();
                }
                else
                {
                    var match = configuration.CustomMapRules.FirstOrDefault(o => o.Key.Equals(context.Request.Path));
                    if (!match.IsNull())
                    {
                        match.Value.Invoke(context);
                    }
                    else
                    {
                        await next();
                    }
                }
            });

            app.UseGearStatusCodeResponse();

            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                //------------------------------------------App events-------------------------------------
                var lifeTimeService = serviceScope.ServiceProvider.GetService<IHostApplicationLifetime>();
                lifeTimeService.RegisterAppEvents(app, configuration.AppName);

                //----------------------------------Localization Usage-------------------------------------

                var languages = serviceScope.ServiceProvider.GetService<IOptionsSnapshot<LocalizationConfig>>();
                app.UseLocalizationModule(languages);

                if (configuration.AutoApplyPendingMigrations)
                {
                    DbContextMigrationTool.ApplyPendingMigrations();
                }
            }

            if (GearApplication.IsHostedOnLinux())
            {
                var forwardOptions = new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                };

                if (configuration.HostingEnvironment.IsDevelopment())
                {
                    forwardOptions.ForwardLimit = 2;
                }

                app.UseForwardedHeaders(forwardOptions);
            }

            if (configuration.HostingEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            if (GearWebApplication.ApplicationArgs.UseKestrelRedirectToHttps)
            {
                var httpsSection = configuration.Configuration.GetSection("HttpServer:Endpoints:Https");
                if (httpsSection.Exists())
                {
                    var httpsEndpoint = new EndpointConfiguration();
                    httpsSection.Bind(httpsEndpoint);
                    app.UseRewriter(new RewriteOptions().AddRedirectToHttps(
                        configuration.HostingEnvironment.IsDevelopment() ? StatusCodes.Status302Found : StatusCodes.Status301MovedPermanently,
                        httpsEndpoint.Port));
                }
            }

            //--------------------------------------Swagger Usage-------------------------------------
            if (configuration.SwaggerConfiguration.UseSwaggerUI &&
                (configuration.SwaggerConfiguration.UseOnlyInDevelopment && configuration.HostingEnvironment.IsDevelopment() || !configuration.SwaggerConfiguration.UseOnlyInDevelopment))
            {
                app.UseSwagger()
                    .UseSwaggerUI(options =>
                    {
                        options.SwaggerEndpoint($"/swagger/v{GearApplication.AppVersion}/swagger.json", $"{GearApplication.ApplicationName} API v{GearApplication.AppVersion}");
                        options.OAuthClientId("clientId");
                        options.OAuthAppName(GearApplication.ApplicationName);
                        options.OAuthUsePkce();
                        options.DisplayRequestDuration();
                        options.EnableDeepLinking();
                    });
            }

            //----------------------------------Static files Usage-------------------------------------
            if (configuration.AppFileConfiguration.UseDefaultFiles)
                app.UseDefaultFiles();

            if (configuration.AppFileConfiguration.UseStaticFile)
            {
                app.UseStaticFiles();
                var moduleContentFolder = Path.Combine(AppContext.BaseDirectory, "wwwroot/_content");
                if (Directory.Exists(moduleContentFolder))
                {
                    var modulesFolders = Directory.GetDirectories(moduleContentFolder);
                    foreach (var moduleFolder in modulesFolders)
                    {
                        app.UseStaticFiles(new StaticFileOptions
                        {
                            FileProvider = new PhysicalFileProvider(moduleFolder),
                            HttpsCompression = Microsoft.AspNetCore.Http.Features.HttpsCompressionMode.Compress,
                            OnPrepareResponse = context =>
                            {
                                var headers = context.Context.Response.GetTypedHeaders();
                                headers.CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
                                {
                                    Public = true,
                                    MaxAge = TimeSpan.FromDays(1)
                                };
                            }
                        });
                    }
                }
            }

            if (configuration.AppFileConfiguration.UseResponseCaching)
                app.UseResponseCaching();

            //--------------------------------------Use compression-------------------------------------
            if (configuration.UseResponseCompression && configuration.HostingEnvironment.IsProduction()) app.UseResponseCompression();


            //----------------------------------Origin Cors Usage-------------------------------------
            if (configuration.UseDefaultCorsConfiguration) app.UseConfiguredCors();

            app.UseRouting();

            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            //custom rules
            app.UseAppMvc(configuration.Configuration, configuration.MvcTemplate);

            return new GearAppBuilder(app.ApplicationServices);
        }

        /// <summary>
        /// Use app as mvc
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configuration"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseAppMvc(this IApplicationBuilder app, IConfiguration configuration, string template)
        {
            var isConfigured = configuration.GetValue<bool>("IsConfigured");

            var mvcTemplate = isConfigured
                ? template
                : "{controller=Installer}/{action=Index}";

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", mvcTemplate);
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });

            return app;
        }
    }
}