using GR.Card.Abstractions.Models;
using GR.Core.Abstractions;
using GR.Core.Extensions;
using GR.Core.Razor.BaseControllers;
using GR.Identity.Abstractions.Helpers.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace GR.Card.Razor.Controllers
{
    [Admin, AutoValidateAntiforgeryToken]
    public class CreditCardSettingsController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject paypal settings
        /// </summary>
        private readonly IWritableOptions<CardSettingsViewModel> _cardSettings;

        #endregion

        public CreditCardSettingsController(IWritableOptions<CardSettingsViewModel> cardSettings)
        {
            _cardSettings = cardSettings;
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
            return View(_cardSettings.Value);
        }

        /// <summary>
        /// Save new config
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Config(CardSettingsViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var updateResponse = _cardSettings.Update(options =>
            {
                options.ApiKey = model.ApiKey;
                options.TransactionKey = model.TransactionKey;
                options.IsSandbox = model.IsSandbox;
            }, "AppSettings/CardSettings.json");
            if (updateResponse.IsSuccess) ViewData["isSuccess"] = true;
            else
            {
                ModelState.AppendResultModelErrors(updateResponse.Errors);
            }
            return View(model);
        }
    }
}