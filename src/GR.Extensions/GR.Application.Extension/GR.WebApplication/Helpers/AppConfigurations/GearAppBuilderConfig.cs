using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace GR.WebApplication.Helpers.AppConfigurations
{
    public class GearAppBuilderConfig
    {
        /// <summary>
        /// App name
        /// </summary>
        public virtual string AppName { get; set; } = "GEAR_APP";

        /// <summary>
        /// Hosting environment
        /// </summary>
        public IHostingEnvironment HostingEnvironment { get; set; }

        /// <summary>
        /// App configuration
        /// </summary>
        public IConfiguration Configuration { get; set; }

        /// <summary>
        /// Use dynamic pages url rewrite
        /// </summary>
        public virtual bool UseCustomUrlRewrite { get; set; } = true;

        /// <summary>
        /// Use default cors 
        /// </summary>
        public virtual bool UseDefaultCorsConfiguration { get; set; } = true;

        /// <summary>
        /// Use Response Compression
        /// </summary>
        public virtual bool UseResponseCompression { get; set; } = true;

        /// <summary>
        /// Swagger configuration
        /// </summary>
        public SwaggerConfiguration SwaggerConfiguration { get; set; } = new SwaggerConfiguration();

        /// <summary>
        /// Web app configuration
        /// </summary>
        public GearAppFileConfiguration AppFileConfiguration { get; set; } = new GearAppFileConfiguration();

        /// <summary>
        /// Signlar configuration
        /// </summary>
        public SignlarAppConfiguration SignlarAppConfiguration { get; set; } = new SignlarAppConfiguration();

        /// <summary>
        /// Custom map rules
        /// </summary>
        public virtual Dictionary<string, Action<HttpContext>> CustomMapRules { get; set; } = new Dictionary<string, Action<HttpContext>>();
    }

    public sealed class SwaggerConfiguration
    {
        public bool UseSwaggerUI { get; set; } = true;
        public bool UseOnlyInDevelopment { get; set; }
    }

    public sealed class GearAppFileConfiguration
    {
        public bool UseDefaultFiles { get; set; } = true;
        public bool UseStaticFile { get; set; } = true;
    }

    public sealed class SignlarAppConfiguration
    {
        public bool UseDefaultSignlarConfiguration { get; set; } = true;

        /// <summary>
        /// Path route
        /// </summary>
        public string Path { get; set; } = "/rtn";
    }
}