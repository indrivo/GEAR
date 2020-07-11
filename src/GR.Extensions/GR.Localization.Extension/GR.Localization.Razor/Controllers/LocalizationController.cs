using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Razor.BaseControllers;
using GR.Identity.Abstractions;
using GR.Localization.Abstractions;
using GR.Localization.Abstractions.Events;
using GR.Localization.Abstractions.Events.EventArgs;
using GR.Localization.Abstractions.Extensions;
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
        private readonly IExternalTranslationProvider _externalTranslationProvider;
        private readonly IUserManager<GearUser> _userManager;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="locConfig"></param>
        /// <param name="localize"></param>
        /// <param name="localizationService"></param>
        /// <param name="externalTranslationProvider"></param>
        /// <param name="userManager"></param>
        public LocalizationController(IOptionsSnapshot<LocalizationConfigModel> locConfig,
            IStringLocalizer localize, ILocalizationService localizationService, IExternalTranslationProvider externalTranslationProvider, IUserManager<GearUser> userManager)
        {
            _locConfig = locConfig;
            _localize = localize;
            _localizationService = localizationService;
            _externalTranslationProvider = externalTranslationProvider;
            _userManager = userManager;
        }

        /// <summary>
        /// Get all languages
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public JsonResult GetAvailablesLanguages()
        {
            var languages = _locConfig.Value.Languages;
            var parsed = new List<dynamic>();
            var result = new ResultModel();
            foreach (var lang in languages)
            {
                parsed.Add(new
                {
                    Locale = lang.Identifier,
                    Description = lang.Name
                });
            }

            result.Result = parsed;

            return Json(result);
        }

        /// <summary>
        /// Get translations
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public JsonResult GetTranslations(string lang)
        {
            var languages = _locConfig.Value.Languages.Select(x => x.Identifier);
            if (!languages.Contains(lang))
            {
                return Json(null);
            }

            var translations = _localize.GetAllForLanguage(lang);
            var json = translations.ToDictionary(trans => trans.Name, trans => trans.Value);
            return Json(json);
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
        /// Translate all keys from english
        /// </summary>
        /// <param name="text"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        public async Task<JsonResult> Translate([Required] string text, [Required] string from)
        {
            var result = new ResultModel
            {
                Errors = new List<IErrorModel>()
            };
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(from))
            {
                result.Errors.Add(new ErrorModel("EmptyKey", "Key is empty!"));
                return Json(result);
            }
            var languages = _locConfig.Value.Languages.ToDictionary(f => f.Identifier, f => f.Name);
            var dict = new Dictionary<string, string>();
            foreach (var (key, _) in languages)
            {
                if (key == from) continue;
                var translated = await _externalTranslationProvider.TranslateTextAsync(text, from, key);
                dict.Add(key, translated);
            }

            result.Result = dict;
            result.IsSuccess = true;
            return Json(result);
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

        /// <summary>
        /// Get all languages
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> GetLanguagesAsJson()
        {
            var languagesRequest = await _localizationService.GetAllLanguagesAsync();
            return Json(languagesRequest.Result);
        }

        /// <summary>
        /// Get translations
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetTranslationsForCurrentLanguage()
        {
            var lang = HttpContext.Session.GetString("lang");
            var languages = _locConfig.Value.Languages.Select(x => x.Identifier);
            if (!languages.Contains(lang))
            {
                return Json(null);
            }

            var translations = _localize.GetAllForLanguage(lang).OrderBy(x => x.Value);
            var json = translations.ToDictionary(trans => trans.Name, trans => trans.Value);
            return Json(json);
        }
    }
}