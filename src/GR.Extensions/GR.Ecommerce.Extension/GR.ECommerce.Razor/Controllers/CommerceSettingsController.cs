using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core;
using GR.ECommerce.Abstractions;
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
            var model = new CommerceSettingsViewModel
            {
                Currencies = (await _productService.GetAllCurrenciesAsync()).Result,
                CurrencyCode = (await _productService.GetSettingAsync<string>(CommerceResources.SettingsParameters.CURRENCY)).Result
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
            return RedirectToAction(nameof(Index));
        }
    }
}