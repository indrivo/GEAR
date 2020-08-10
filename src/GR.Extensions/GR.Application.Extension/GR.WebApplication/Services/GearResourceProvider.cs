using GR.Core.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace GR.WebApplication.Services
{
    public class GearResourceProvider : IGearResourceProvider
    {
        #region Injectable

        /// <summary>
        /// Inject host env
        /// </summary>
        private readonly IWebHostEnvironment _hostEnvironment;

        #endregion

        public GearResourceProvider(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        /// <summary>
        /// Get app settings path
        /// </summary>
        /// <returns></returns>
        public string AppSettingsFilepath()
        {
            var path = "appsettings.json";
            if (_hostEnvironment.IsDevelopment())
            {
                path = "appsettings.Development.json";
            }
            else if (_hostEnvironment.IsEnvironment("Stage"))
            {
                path = "appsettings.Stage.json";
            }
            return path;
        }
    }
}