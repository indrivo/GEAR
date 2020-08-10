﻿using System;
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
        /// Mvc start view template
        /// </summary>
        public virtual string MvcTemplate { get; set; } = "{controller=Home}/{action=Index}";

        /// <summary>
        /// Hosting environment
        /// </summary>
        public IWebHostEnvironment HostingEnvironment { get; set; }

        /// <summary>
        /// App configuration
        /// </summary>
        public IConfiguration Configuration { get; set; }

        /// <summary>
        /// Use default cors 
        /// </summary>
        public virtual bool UseDefaultCorsConfiguration { get; set; } = true;

        /// <summary>
        /// Use Response Compression
        /// </summary>
        public virtual bool UseResponseCompression { get; set; } = true;

        /// <summary>
        /// Use health check
        /// </summary>
        public virtual bool UseHealthCheck { get; set; } = true;

        /// <summary>
        /// Apply auto migrations
        /// </summary>
        public virtual bool AutoApplyPendingMigrations { get; set; } = false;

        /// <summary>
        /// Swagger configuration
        /// </summary>
        public SwaggerConfiguration SwaggerConfiguration { get; set; } = new SwaggerConfiguration();

        /// <summary>
        /// Web app configuration
        /// </summary>
        public GearAppFileConfiguration AppFileConfiguration { get; set; } = new GearAppFileConfiguration();

        /// <summary>
        /// Custom map rules
        /// </summary>
        public virtual Dictionary<string, Action<HttpContext>> CustomMapRules { get; set; } = new Dictionary<string, Action<HttpContext>>();
    }

    public sealed class SwaggerConfiguration
    {
        // ReSharper disable once InconsistentNaming
        public bool UseSwaggerUI { get; set; } = true;
        public bool UseOnlyInDevelopment { get; set; }
    }

    public sealed class GearAppFileConfiguration
    {
        public bool UseDefaultFiles { get; set; } = true;
        public bool UseStaticFile { get; set; } = true;
        public bool UseResponseCaching { get; set; } = true;
    }
}