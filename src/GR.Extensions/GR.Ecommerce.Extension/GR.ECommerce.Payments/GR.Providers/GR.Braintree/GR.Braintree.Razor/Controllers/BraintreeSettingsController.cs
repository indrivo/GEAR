using GR.Braintree.Abstractions.Models;
using GR.Core.Abstractions;
using GR.Core.Extensions;
using GR.Identity.Abstractions.Helpers.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace GR.Braintree.Razor.Controllers
{
    [AutoValidateAntiforgeryToken]
    [Admin]
    public class BraintreeSettingsController : Controller
    {
        #region Injectable

        private readonly IWritableOptions<BraintreeConfiguration> _braintreeOptions;

        #endregion

        public BraintreeSettingsController(IWritableOptions<BraintreeConfiguration> braintreeOptions)
        {
            _braintreeOptions = braintreeOptions;
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
            return View(_braintreeOptions.Value);
        }

        /// <summary>
        /// Save new config
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Config(BraintreeConfiguration model)
        {
            if (!ModelState.IsValid) return View(model);
            var updateResponse = _braintreeOptions.Update(options =>
            {
                options.IsProduction = model.IsProduction;
                options.MerchantId = model.MerchantId;
                options.PrivateKey = model.PrivateKey;
                options.PublicKey = model.PublicKey;
            }, "AppSettings/BraintreeSettings.json");
            if (updateResponse.IsSuccess) ViewData["isSuccess"] = true;
            else
            {
                ModelState.AppendResultModelErrors(updateResponse.Errors);
            }
            return View(model);
        }
    }
}