using System;
using System.Collections.Generic;
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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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


            GearApplication.SetAppName(configuration.AppName);

            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var sp = serviceScope.ServiceProvider;
                var environment = sp.GetService<IHostingEnvironment>();
                GearWebApplication.IsConfigured(environment);

                //------------------------------------------App events-------------------------------------
                var lifeTimeService = serviceScope.ServiceProvider.GetService<IApplicationLifetime>();
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

            //----------------------------------Origin Cors Usage-------------------------------------
            if (configuration.UseDefaultCorsConfiguration) app.UseConfiguredCors();

            app.UseCookiePolicy();
            app.UseAuthentication();

            //custom rules
            app.UseAppMvc(configuration.Configuration, configuration.MvcTemplate, configuration.CustomMapRules);

            //--------------------------------------Swagger Usage-------------------------------------
            if (configuration.SwaggerConfiguration.UseSwaggerUI &&
                (configuration.SwaggerConfiguration.UseOnlyInDevelopment && configuration.HostingEnvironment.IsDevelopment() || !configuration.SwaggerConfiguration.UseOnlyInDevelopment))
            {
                app.UseSwagger()
                    .UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "GEAR API v1.0");
                    });
            }

            //----------------------------------Static files Usage-------------------------------------
            if (configuration.AppFileConfiguration.UseDefaultFiles)
                app.UseDefaultFiles();

            if (configuration.AppFileConfiguration.UseStaticFile)
                app.UseStaticFiles();

            if (configuration.AppFileConfiguration.UseResponseCaching)
                app.UseResponseCaching();

            //--------------------------------------Use compression-------------------------------------
            if (configuration.UseResponseCompression && configuration.HostingEnvironment.IsProduction()) app.UseResponseCompression();

            return new GearAppBuilder(app.ApplicationServices);
        }

        /// <summary>
        /// Use app as mvc
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configuration"></param>
        /// <param name="template"></param>
        /// <param name="routeMapping"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseAppMvc(this IApplicationBuilder app, IConfiguration configuration, string template, Dictionary<string, Action<HttpContext>> routeMapping = null)
        {
            var isConfigured = configuration.GetValue<bool>("IsConfigured");

            var mvcTemplate = isConfigured
                ? template
                : "{controller=Installer}/{action=Index}";

            app.UseMvc(routes =>
            {
                routes.ApplicationBuilder.Use(async (context, next) =>
                {
                    if (routeMapping == null || !isConfigured)
                    {
                        await next();
                    }
                    else
                    {
                        var match = routeMapping.FirstOrDefault(o => o.Key.Equals(context.Request.Path));
                        if (!match.IsNull())
                        {
                            match.Value.Invoke(context);
                            await next();
                        }
                        else
                        {
                            await next();
                        }
                    }
                });

                routes.MapRoute(
                    name: "default",
                    template: mvcTemplate,
                    defaults: mvcTemplate
                );
            });
            return app;
        }
    }
}