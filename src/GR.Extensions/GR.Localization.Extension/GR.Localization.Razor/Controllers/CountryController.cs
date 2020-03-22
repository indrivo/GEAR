using System.Threading.Tasks;
using GR.Core;
using GR.Identity.Abstractions.Helpers.Attributes;
using GR.Localization.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR.Localization.Razor.Controllers
{
    [Authorize]
    [Roles(GlobalResources.Roles.ADMINISTRATOR)]
    public class CountryController : Controller
    {
        #region Injectable

        /// <summary>
        /// Inject location service
        /// </summary>
        private readonly ICountryService _locationService;

        #endregion Injectable

        public CountryController(ICountryService locationService)
        {
            _locationService = locationService;
        }


        /// <summary>
        /// Get countries View
        /// </summary>
        /// <returns></returns>
        public IActionResult Countries()
        {
            return View();
        }

        public async Task<IActionResult> Cities(string id)
        {
            var countryRequest = await _locationService.GetCountryByIdAsync(id);
            if (!countryRequest.IsSuccess) return NotFound();
            ViewBag.Country = countryRequest.Result;
            return View();
        }
    }
}
