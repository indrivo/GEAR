using GR.Core.Abstractions;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers.Global;
using GR.GooglePay.Abstractions.ViewModels;
using GR.Identity.Abstractions.Helpers.Attributes;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace GR.GooglePay.Razor.Controllers
{
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    [Admin]
    [AutoValidateAntiforgeryToken]
    public class GPaySettingsController : Controller
    {
        #region Injectable

        /// <summary>
        /// Inject google pay settings
        /// </summary>
        private readonly IWritableOptions<GPaySettingsViewModel> _writableOptions;

        #endregion

        public GPaySettingsController(IWritableOptions<GPaySettingsViewModel> writableOptions)
        {
            _writableOptions = writableOptions;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction("Config");
        }

        /// <summary>
        /// Get config view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Config()
        {
            return View(_writableOptions.Value.Adapt<GPayEditSettingsViewModel>());
        }

        /// <summary>
        /// Save new config
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Config(GPayEditSettingsViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var updateResponse = _writableOptions.Update(options =>
            {
                options.MerchantId = model.MerchantId;
                options.IsSandbox = model.IsSandbox;
                options.AllowedCardAuthMethods = model.AllowedCardAuthMethods;
                options.AllowedCardNetworks = model.AllowedCardNetworks;
                options.MerchantName = model.MerchantName;
                options.ApiVersion = model.ApiVersion;
            }, "AppSettings/GPaySettings.json");
            if (updateResponse.IsSuccess) ViewData["isSuccess"] = true;
            else
            {
                ModelState.AppendResultModelErrors(updateResponse.Errors);
            }
            return View(model);
        }
    }
}