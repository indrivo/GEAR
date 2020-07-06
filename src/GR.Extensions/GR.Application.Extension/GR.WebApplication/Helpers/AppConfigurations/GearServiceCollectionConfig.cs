﻿using System;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers.Global;
using GR.WebApplication.Helpers.Versioning;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GR.WebApplication.Helpers.AppConfigurations
{
    [Author(Authors.LUPEI_NICOLAE)]
    public class GearServiceCollectionConfig
    {
        /// <summary>
        /// Use hot reload for razor class libraries.
        /// Requirements:
        /// 1. Development env and debug, debugger need to be attached.
        /// 2. All razor class library projects must end with: *.Razor.
        /// 3. Source not published
        /// If all requirements are respected, the changed views are recompiled
        /// </summary>
        public bool UseHotReload { get; set; }

        /// <summary>
        /// Use health check
        /// </summary>
        public bool UseHealthCheck { get; set; } = true;

        private static IServiceProvider BuildServices { get; set; }
        public IServiceProvider BuildGearServices => BuildServices ?? (BuildServices = GearServices.BuildServiceProvider());

        /// <summary>
        /// Hosting environment
        /// </summary>
        public virtual IHostingEnvironment HostingEnvironment { get; set; }

        /// <summary>
        /// App configuration
        /// </summary>
        public virtual IConfiguration Configuration { get; set; }

        /// <summary>
        /// Gear services
        /// </summary>
        public virtual IServiceCollection GearServices { get; set; }

        /// <summary>
        /// Use Cors default configuration
        /// </summary>
        public virtual bool UseDefaultCorsConfiguration { get; set; } = true;

        /// <summary>
        /// Add response compression
        /// </summary>
        public virtual bool AddResponseCompression { get; set; } = true;

        /// <summary>
        /// Cache configuration
        /// </summary>
        public virtual CacheConfiguration CacheConfiguration { get; set; } = new CacheConfiguration();

        /// <summary>
        /// Api Version Options
        /// </summary>
        public virtual ApiVersioningOptions ApiVersionOptions { get; set; } = new ApiVersioningOptions
        {
            ReportApiVersions = true,
            AssumeDefaultVersionWhenUnspecified = true,
            DefaultApiVersion = new ApiVersion(1, 0),
            ErrorResponses = new UnsupportedApiVersionErrorResponseProvider()
        };

        /// <summary>
        /// Swagger configuration
        /// </summary>
        public virtual SwaggerServicesConfiguration SwaggerServicesConfiguration { get; set; } = new SwaggerServicesConfiguration();

        /// <summary>
        /// Server configuration
        /// </summary>
        public virtual ServerConfiguration ServerConfiguration { get; set; } = new ServerConfiguration();
    }

    public sealed class CacheConfiguration
    {
        /// <summary>
        /// Use distributed cache
        /// </summary>
        public bool UseDistributedCache { get; set; } = false;

        /// <summary>
        /// Use in memory cache
        /// </summary>
        public bool UseInMemoryCache { get; set; } = true;
    }

    public sealed class SwaggerServicesConfiguration
    {
        public bool UseSwagger { get; set; } = true;
        public bool UseDefaultConfiguration { get; set; } = true;
    }

    public sealed class ServerConfiguration
    {
        /// <summary>
        /// Represent max size on upload data
        /// </summary>
        public int UploadMaximSize { get; set; } = int.MaxValue;
    }
}
