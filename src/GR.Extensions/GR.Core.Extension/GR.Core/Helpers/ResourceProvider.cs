using Microsoft.Extensions.Hosting;

namespace GR.Core.Helpers
{
    public static class ResourceProvider
    {
        /// <summary>
        /// Get file path
        /// </summary>
        public static string AppSettingsFilepath(IHostEnvironment hostingEnvironment)
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
