using GR.Core.Abstractions;
using GR.Core.Extensions;
using GR.Identity.Abstractions.Helpers.Attributes;
using GR.Paypal.Models;
using Microsoft.AspNetCore.Mvc;

namespace GR.Paypal.Razor.Controllers
{
    [Admin, AutoValidateAntiforgeryToken]
    public class PaypalSettingsController : Controller
    {
        #region Injectable

        /// <summary>
        /// Inject paypal settings
        /// </summary>
        private readonly IWritableOptions<PaypalExpressConfigForm> _payPalOptions;

        #endregion

        public PaypalSettingsController(IWritableOptions<PaypalExpressConfigForm> payPalOptions)
        {
            _payPalOptions = payPalOptions;
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
            return View(_payPalOptions.Value);
        }

        /// <summary>
        /// Save new config
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Config(PaypalExpressConfigForm model)
        {
            if (!ModelState.IsValid) return View(model);
            var updateResponse = _payPalOptions.Update(options =>
            {
                options.ClientId = model.ClientId;
                options.ClientSecret = model.ClientSecret;
                options.IsSandbox = model.IsSandbox;
                options.PaymentFee = model.PaymentFee;
            }, "AppSettings/PaypalSettings.json");
            if (updateResponse.IsSuccess) ViewData["isSuccess"] = true;
            else
            {
                ModelState.AppendResultModelErrors(updateResponse.Errors);
            }
            return View(model);
        }
    }
}