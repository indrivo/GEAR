﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using ST.Report.Dynamic.Razor.Helpers;

namespace ST.Report.Dynamic.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register ui module
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddReportUIModule(this IServiceCollection services)
        {
            services.ConfigureOptions(typeof(ReportFileConfiguration));
            return services;
        }
    }
}
