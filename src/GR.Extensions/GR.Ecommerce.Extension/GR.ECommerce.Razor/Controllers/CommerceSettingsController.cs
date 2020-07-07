using System;
using System.Threading.Tasks;
using GR.Core;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Enums;
using GR.ECommerce.Abstractions.Helpers;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Razor.ViewModels.SettingsViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR.ECommerce.Razor.Controllers
{
    [Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
    public sealed class CommerceSettingsController : Controller
    {
        #region Injectable

        /// <summary>
        /// Inject product service
        /// </summary>
        private readonly IProductService<Product> _productService;

        #endregion

        public CommerceSettingsController(IProductService<Product> productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Commerce settings
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var daysNotifySubscription = (await _productService.GetSettingAsync<string>(CommerceResources.SettingsParameters.DAYS_NOTIFY_SUBSCRIPTION_EXPIRATION)).Result ?? "0";
            var dayFreeTrialPeriod = (await _productService.GetSettingAsync<string>(CommerceResources.SettingsParameters.FREE_TRIAL_PERIOD_DAYS)).Result ?? "15";

            var model = new CommerceSettingsViewModel
            {
                Currencies = (await _productService.GetAllCurrenciesAsync()).Result,
                CurrencyCode = (await _productService.GetSettingAsync<string>(CommerceResources.SettingsParameters.CURRENCY)).Result,
                DaysToNotifyExpiringSubscriptions = Convert.ToInt32(daysNotifySubscription),
                DaysToFreeTrialPeriod = Convert.ToInt32(dayFreeTrialPeriod),
            };

            return View(model);
        }

        /// <summary>
        /// Update settings
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveCommerceSettings(CommerceSettingsViewModel model)
        {
            await _productService.AddOrUpdateSettingAsync(CommerceResources.SettingsParameters.CURRENCY, model.CurrencyCode);
            await _productService.AddOrUpdateSettingAsync(CommerceResources.SettingsParameters.DAYS_NOTIFY_SUBSCRIPTION_EXPIRATION,
                model.DaysToNotifyExpiringSubscriptions, CommerceSettingType.Number);

            await _productService.AddOrUpdateSettingAsync(CommerceResources.SettingsParameters.FREE_TRIAL_PERIOD_DAYS,
                model.DaysToFreeTrialPeriod, CommerceSettingType.Number);

            return RedirectToAction(nameof(Index));
        }
    }
}