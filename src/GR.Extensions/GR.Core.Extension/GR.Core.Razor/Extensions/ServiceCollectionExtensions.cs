using System;
using System.Collections.Generic;
using System.IO;
using GR.Core.Razor.Helpers;
using GR.Core.Razor.Helpers.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;

namespace GR.Core.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add swagger
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddSwaggerModule(this IServiceCollection services, IConfiguration configuration)
        {
            var confAuth = configuration.GetSection("WebClients").GetSection("CORE");
            var authUrl = confAuth.GetValue<string>("uri");
            services.AddSwaggerGen(options =>
            {
                options.DocInclusionPredicate(SwaggerVersioning.DocInclusionPredicate);

                options.SwaggerDoc("v1.0", new Info
                {
                    Title = "GEAR APP HTTP API",
                    Version = "v1.0",
                    Description = "GEAR Service HTTP API",
                    TermsOfService = "Terms Of Service",
                    Contact = new Contact
                    {
                        Url = "http://indrivo.com",
                        Email = "support@indrivo.com",
                        Name = "Indrivo SRL"
                    }
                });

                // Set this flag to omit descriptions for any actions decorated with the Obsolete attribute
                //
                options.IgnoreObsoleteActions();

                // Set this flag to omit schema property descriptions for any type properties decorated with the
                // Obsolete attribute
                //
                options.IgnoreObsoleteProperties();

                // Set custom schema name 
                options.CustomSchemaIds(x => x.FullName);

                // In accordance with the built in JsonSerializer, Swashbuckle will, by default, describe enums as integers.
                // You can change the serializer behavior by configuring the StringToEnumConverter globally or for a given
                // enum type. Swashbuckle will honor this change out-of-the-box. However, if you use a different
                // approach to serialize enums as strings, you can also force Swashbuckle to describe them as strings.
                //
                options.DescribeAllEnumsAsStrings();
                options.DescribeStringEnumsInCamelCase();
                options.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Type = "oauth2",
                    Flow = "implicit",
                    AuthorizationUrl = $"{authUrl}/connect/authorize",
                    TokenUrl = $"{authUrl}/connect/token",
                    Scopes = new Dictionary<string, string>
                    {
                        {"core", "CORE API"}
                    }
                });
                options.OperationFilter<SwaggerAuthorizeCheckOperationFilter>();
                // Integrate XML comments
                var xmlCommentsFiles = GetXmlCommentsPaths();
                foreach (var xmlCommentsFile in xmlCommentsFiles)
                {
                    options.IncludeXmlComments(xmlCommentsFile);
                }
            });
            return services;
        }

        /// <summary>
        /// Add configured Cors
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddOriginCorsModule(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", b =>
                {
                    b.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            return services;
        }

        /// <summary>
        /// Use Cors
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseConfiguredCors(this IApplicationBuilder app)
        {
            app.UseCors("CorsPolicy")
                .UseStaticFiles()
                .UseSession();
            return app;
        }

        /// <summary>
        /// Get xml comments
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<string> GetXmlCommentsPaths()
        {
            var basePath = PlatformServices.Default.Application.ApplicationBasePath;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var fileName = assembly.GetName().Name + ".xml";
                var path = Path.Combine(basePath, fileName);
                if (File.Exists(path)) yield return path;
            }
        }
    }
}
