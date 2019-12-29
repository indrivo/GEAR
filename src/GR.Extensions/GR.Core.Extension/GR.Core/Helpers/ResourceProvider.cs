using Microsoft.AspNetCore.Hosting;

namespace GR.Core.Helpers
{
    public static class ResourceProvider
    {
        /// <summary>
        /// Get file path
        /// </summary>
        public static string AppSettingsFilepath(IHostingEnvironment hostingEnvironment)
        {
            var path = "appsettings.json";
            if (hostingEnvironment.IsDevelopment())
            {
                path = "appsettings.Development.json";
            }
            else if (hostingEnvironment.IsEnvironment("Stage"))
            {
                path = "appsettings.Stage.json";
            }
            return path;
        }
    }
}
