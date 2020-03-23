using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GR.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Identity.Abstractions.Helpers;
using GR.Identity.Abstractions.ViewModels.SeedViewModels;
using GR.Install.Abstractions;
using GR.Install.Abstractions.Models;
using GR.WebApplication;
using Microsoft.Extensions.Configuration;

namespace GR.Install.Razor.Controllers
{
    [AllowAnonymous]
    public class InstallerController : Controller
    {
        #region Injectable

        /// <summary>
        /// Inject configuration
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Inject installer service
        /// </summary>
        private readonly IGearWebInstallerService _installerService;

        #endregion

        /// <summary>
        /// Is system configured
        /// </summary>
        private readonly bool _isConfigured;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="hostingEnvironment"></param>
        /// <param name="configuration"></param>
        /// <param name="installerService"></param>
        public InstallerController(IHostingEnvironment hostingEnvironment, IConfiguration configuration, IGearWebInstallerService installerService)
        {
            _configuration = configuration;
            _installerService = installerService;
            _isConfigured = GearWebApplication.IsConfigured(hostingEnvironment);
        }

        /// <summary>
        /// Load setup page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Setup()
        {
            if (_isConfigured) return RedirectToAction("Index", "Home");
            var model = new SetupModel();
            var (provider, connectionString) = _configuration.GetConnectionStringInfo();
            model.DataBaseType = provider;
            model.DatabaseConnectionString = connectionString;
            var baseDirectory = AppContext.BaseDirectory;
            var data = JsonParser.ReadObjectDataFromJsonFile<IdentitySeedViewModel>(Path.Combine(baseDirectory,
                IdentityResources.Configuration.DEFAULT_FILE_PATH));
            var user = data.ApplicationUsers.FirstOrDefault();
            if (user == null) throw new Exception();
            model.SysAdminProfile = new SetupProfileModel
            {
                FirstName = "admin",
                Email = user.Email,
                LastName = "admin",
                Password = user.Password,
                ConfirmPassword = user.Password,
                UserName = user.UserName
            };

            model.Organization = new SetupOrganizationViewModel
            {
                Name = "Indrivo"
            };

            return View(model);
        }

        /// <summary>
        /// Complete installation of system
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Setup(SetupModel model)
        {
            var installResponse = await _installerService.InstallAsync(model);
            if (installResponse.IsSuccess) return RedirectToAction("Index", "Home");
            ModelState.AppendResultModelErrors(installResponse.Errors);
            return View(model);
        }

        /// <summary>
		/// Load welcome page
		/// </summary>
		/// <returns></returns>
		public IActionResult Index()
        {
            if (_isConfigured) return RedirectToAction("Index", "Home");
            GearApplication.AppState.Installed = false;
            return View();
        }
    }
}