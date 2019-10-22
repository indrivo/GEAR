using Microsoft.AspNetCore.Mvc;
using ST.Core.Abstractions;
using ST.ECommerce.Paypal.Impl.Models;
using ST.ECommerce.Paypal.Razor.ViewModels;

namespace ST.ECommerce.Paypal.Razor.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class PaypalSettingsController : Controller
    {
        private readonly IWritableOptions<PaypalExpressConfigForm> _payPalOptions;

        public PaypalSettingsController(IWritableOptions<PaypalExpressConfigForm> payPalOptions)
        {
            _payPalOptions = payPalOptions;
        }

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
            _payPalOptions.Update(options =>
            {
                options.ClientId = model.ClientId;
                options.ClientSecret = model.ClientSecret;
                options.IsSandbox = model.IsSandbox;
            });
            ViewData["isSuccess"] = true;
            return View(model);
        }
    }
}