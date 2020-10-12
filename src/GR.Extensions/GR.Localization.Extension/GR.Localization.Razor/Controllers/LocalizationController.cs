using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using GR.Core.Extensions;
using GR.Core.Razor.BaseControllers;
using GR.Identity.Abstractions;
using GR.Localization.Abstractions;
using GR.Localization.Abstractions.Events;
using GR.Localization.Abstractions.Events.EventArgs;
using GR.Localization.Abstractions.ViewModels.LocalizationViewModels;

namespace GR.Localization.Razor.Controllers
{
    [Authorize]
    public class LocalizationController : BaseGearController
    {
        #region Injectable

        private readonly IOptionsSnapshot<LocalizationConfigModel> _locConfig;
        private readonly IStringLocalizer _localize;
        private readonly ILocalizationService _localizationService;
        private readonly IUserManager<GearUser> _userManager;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="locConfig"></param>
        /// <param name="localize"></param>
        /// <param name="localizationService"></param>
        /// <param name="userManager"></param>
        public LocalizationController(IOptionsSnapshot<LocalizationConfigModel> locConfig,
            IStringLocalizer localize, ILocalizationService localizationService, IUserManager<GearUser> userManager)
        {
            _locConfig = locConfig;
            _localize = localize;
            _localizationService = localizationService;
            _userManager = userManager;
        }

        /// <summary>
        /// Set current language of the website.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ChangeLanguage(string identifier)
        {
            if (!string.IsNullOrEmpty(identifier))
            {
                var sessionKey = _locConfig.Value.SessionStoreKeyName;

                // Get the list of supported language identifiers.
                var cLangs = _locConfig.Value.Languages.Select(f => f.Identifier);

                // Check wether the provided language identifier is contained in the list of supported languages.
                // If the language is invalid then set to default language.
                var langIsValid = cLangs.Contains(identifier);
                HttpContext.Session.SetString(sessionKey, langIsValid ? identifier : _locConfig.Value.DefaultLanguage);
                HttpContext.Response.Cookies.Append("language", _locConfig.Value.Languages.FirstOrDefault(x => x.Identifier == identifier)?.Name);

                LocalizationEvents.Languages.ChangeLanguage(new ChangeLanguageEventArgs
                {
                    Identifier = identifier,
                    UserId = _userManager.FindUserIdInClaims().Result
                });
            }

            var referer = Request.Headers["Referer"].ToString();
            return Redirect(string.IsNullOrEmpty(referer) ? Url.Action("Index", "Home") : referer);
        }

        /// <summary>
        /// Get languages
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetLanguages()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get key for edit
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult EditKey(string key)
        {
            var config = _localizationService.GetKeyConfiguration(key);
            if (!config.IsSuccess) return NotFound();
            return View(config.Result);
        }

        /// <summary>
        /// Edit keys
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> EditKey(EditLocalizationViewModel model)
        {
            var editResponse = await _localizationService.EditKeyAsync(model);
            if (editResponse.IsSuccess)
                return RedirectToAction("Index", "Localization");
            ModelState.AppendResultModelErrors(editResponse.Errors);
            return View(model);
        }

        [HttpGet]
        public IActionResult AddLanguage() => View();

        /// <summary>
        /// Add key
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult AddKey()
        {
            var conf = _localizationService.GetAddKeyConfiguration();
            return View(conf.Result);
        }

        /// <summary>
        /// Add new key
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddKey(AddKeyViewModel model)
        {
            if (ModelState.IsValid)
            {
                var isValid = model.LocalizedStrings.All(x => !string.IsNullOrEmpty(x.Value));

                if (!isValid)
                {
                    ModelState.AddModelError(string.Empty, "Empty translations");
                }
                else
                if (!_localize[model.NewKey].ResourceNotFound)
                {
                    ModelState.AddModelError(string.Empty, "Key already exists");
                }
                else
                {
                    var addResponse = await _localizationService.AddOrUpdateKeyAsync(model.NewKey, model.LocalizedStrings);
                    if (addResponse.IsSuccess)
                        return RedirectToAction("Index", "Localization");
                    model.Languages = _locConfig.Value.Languages.ToDictionary(f => f.Identifier, f => f.Name);
                    ModelState.AppendResultModelErrors(addResponse.Errors);
                    return View(model);
                }
            }

            if (model.Languages == null)
            {
                model.Languages = _locConfig.Value.Languages.ToDictionary(f => f.Identifier, f => f.Name);
            }

            return View(model);
        }

        /// <summary>
        /// Add language
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddLanguage(AddLanguageViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _localizationService.AddLanguageAsync(model);
                if (response.IsSuccess)
                {
                    return RedirectToAction("GetLanguages", "Localization");
                }

                ModelState.AppendResultModelErrors(response.Errors);
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "Something is not right");

            return View(model);
        }

        /// <summary>
        /// Get language for change status
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ChangeStatus(string key)
        {
            if (key == null) return RedirectToAction("GetLanguages", "Localization", new { page = 1, perPage = 10 });
            var language =
                _locConfig.Value.Languages.Single(x =>
                    string.Equals(x.Identifier, key, StringComparison.CurrentCultureIgnoreCase));
            return View(language);

        }

        /// <summary>
        /// Change status of language
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(LanguageCreateViewModel model)
        {
            var response = await _localizationService.ChangeStatusOfLanguageAsync(model);
            if (!response.IsSuccess)
            {
                foreach (var err in response.Errors)
                {
                    ModelState.AddModelError(err.Key, err.Message);
                }

                return View(model);
            }

            return RedirectToAction("GetLanguages", "Localization", new { page = 1, perPage = 10 });
        }
    }
}