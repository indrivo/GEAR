using System;
using System.Threading.Tasks;
using HealthChecks.UI.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace GR.WebApplication.Helpers
{
    public static class GearHealthCheckWriter
    {
        private const string DEFAULT_CONTENT_TYPE = "application/json";

        public static Task WriteHealthCheckUiResponse(HttpContext httpContext, HealthReport report)
        {
            return WriteHealthCheckUiResponse(httpContext, report, null);
        }

        public static Task WriteHealthCheckUiResponse(
            HttpContext httpContext,
            HealthReport report,
            Action<JsonSerializerSettings> jsonConfigurator)
        {
            var text = "{}";
            httpContext.Response.ContentType = DEFAULT_CONTENT_TYPE;
            if (report == null) return httpContext.Response.WriteAsync(text);
            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            jsonConfigurator?.Invoke(settings);
            settings.Converters.Add(new StringEnumConverter());
            httpContext.Response.ContentType = DEFAULT_CONTENT_TYPE;
            try
            {
                var uiReport = UIHealthReport.CreateFrom(report);
                text = JsonConvert.SerializeObject(uiReport, settings);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return httpContext.Response.WriteAsync(text);
        }
    }
}