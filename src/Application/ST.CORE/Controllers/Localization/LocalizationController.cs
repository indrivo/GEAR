using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using ST.BaseBusinessRepository;
using ST.Configuration.Abstractions;
using ST.Configuration.Server;
using ST.Configuration.ViewModels.LocalizationViewModels;
using ST.CORE.ViewModels;
using ST.Localization;

namespace ST.CORE.Controllers.Localization
{
	[Authorize]
	public class LocalizationController : Controller
	{
		private readonly IOptionsSnapshot<LocalizationConfigModel> _locConfig;
		private readonly IStringLocalizer _localizer;
		private readonly ILocalizationService _localizationService;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="locConfig"></param>
		/// <param name="localizer"></param>
		/// <param name="localizationService"></param>
		public LocalizationController(IOptionsSnapshot<LocalizationConfigModel> locConfig,
			IStringLocalizer localizer, ILocalizationService localizationService)
		{
			_locConfig = locConfig;
			_localizer = localizer;
			_localizationService = localizationService;
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

			var translations = _localizer.GetAllForLanguage(lang);
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
			}

			var referer = Request.Headers["Referer"].ToString();
			return Redirect(string.IsNullOrEmpty(referer) ? Url.Action("Index", "Home") : referer);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IActionResult GetLanguages()
		{
			return View();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="param"></param>
		/// <returns></returns>

		[HttpPost]
		public JsonResult KeysList(DTParameters param)
		{
			var filtered = GetListOfKeysFiltered(param.Search.Value, param.SortOrder, param.Start, param.Length,
				out var totalCount);
			var finalResult = new DTResult<LocalizedString>
			{
				draw = param.Draw,
				data = filtered.ToList(),
				recordsFiltered = totalCount,
				recordsTotal = filtered.Count
			};

			return Json(finalResult);
		}

		/// <summary>
		/// Get keys filtered
		/// </summary>
		/// <param name="search"></param>
		/// <param name="sortOrder"></param>
		/// <param name="start"></param>
		/// <param name="length"></param>
		/// <param name="totalCount"></param>
		/// <returns></returns>
		private List<LocalizedString> GetListOfKeysFiltered(string search, string sortOrder, int start, int length,
			out int totalCount)
		{
			var result = _localizer.GetAllStrings().Where(p =>
				search == null || p.Name != null &&
				p.Name.ToLower().Contains(search.ToLower()) ||
				p.Value != null && p.Value.ToLower().Contains(search.ToLower())).ToList();
			totalCount = result.Count;

			result = result.Skip(start).Take(length).ToList();
			switch (sortOrder)
			{
				case "name":
					result = result.OrderBy(a => a.Name).ToList();
					break;
				case "value":
					result = result.OrderBy(a => a.Value).ToList();
					break;
				case "name DESC":
					result = result.OrderByDescending(a => a.Name).ToList();
					break;
				case "value DESC":
					result = result.OrderByDescending(a => a.Value).ToList();
					break;
				default:
					result = result.AsQueryable().ToList();
					break;
			}

			return result.ToList();
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		private List<LanguageCreateViewModel> GetLanguagesFiltered(string search, string sortOrder, int start,
			int length,
			out int totalCount)
		{
			var result = _locConfig.Value.Languages.Where(p =>
				search == null || p.Name != null &&
				p.Name.ToLower().Contains(search.ToLower()) || p.Identifier != null &&
				p.Identifier.ToLower().Contains(search.ToLower())).ToList();
			totalCount = result.Count;
			switch (sortOrder)
			{
				case "name":
					result = result.OrderBy(a => a.Name).ToList();
					break;
				case "identifier":
					result = result.OrderBy(a => a.Identifier).ToList();
					break;
				case "IsDisabled":
					result = result.OrderBy(a => a.IsDisabled).ToList();
					break;
				case "name DESC":
					result = result.OrderByDescending(a => a.Name).ToList();
					break;
				case "identifier DESC":
					result = result.OrderByDescending(a => a.Identifier).ToList();
					break;
				case "IsDisabled DESC":
					result = result.OrderByDescending(a => a.IsDisabled).ToList();
					break;

				default:
					result = result.AsQueryable().ToList();
					break;
			}

			return result.ToList();
		}

		[HttpPost]
		public JsonResult LanguageList(DTParameters param)
		{
			var filtered = GetLanguagesFiltered(param.Search.Value, param.SortOrder, param.Start, param.Length,
				out var totalCount);
			var finalResult = new DTResult<LanguageCreateViewModel>
			{
				draw = param.Draw,
				data = filtered.ToList(),
				recordsFiltered = totalCount,
				recordsTotal = filtered.Count
			};

			return Json(finalResult);
		}

		/// <summary>
		/// Get key for edit
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		[HttpGet]
		public IActionResult EditKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return View();
			}

			var rz = new Dictionary<string, string>();
			foreach (var item in _locConfig.Value.Languages)
			{
				var str = _localizer.GetForLanguage(key, item.Identifier);
				rz.Add(item.Identifier, str.Value != $"[{str.Name}]" ? str : string.Empty);
			}

			var model = new EditLocalizationViewModel
			{
				Key = key,
				LocalizedStrings = rz,
				Languages = _locConfig.Value.Languages.ToDictionary(f => f.Identifier, f => f.Name)
			};
			return View(model);
		}

		/// <summary>
		/// Edit keys
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult EditKey(EditLocalizationViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			_localizationService.EditKey(model);
			return RedirectToAction("Index", "Localization", new {page = 1, perPage = 10});
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
			var rz = new Dictionary<string, string>();
			foreach (var item in _locConfig.Value.Languages)
			{
				rz.Add(item.Identifier, string.Empty);
			}

			var model = new AddKeyViewModel
			{
				LocalizedStrings = rz,
				Languages = _locConfig.Value.Languages.ToDictionary(f => f.Identifier, f => f.Name)
			};
			return View(model);
		}

		/// <summary>
		/// Add new key
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult AddKey(AddKeyViewModel model)
		{
			if (ModelState.IsValid)
			{
				if (!_localizer[model.NewKey].ResourceNotFound)
				{
					ModelState.AddModelError(string.Empty, "Key already exists");
				}
				else
				{
					_localizationService.AddOrUpdateKey(model.NewKey, model.LocalizedStrings);
					return RedirectToAction("Index", "Localization", new {page = 1, perPage = 10});
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
		public IActionResult AddLanguage(AddLanguageViewModel model)
		{
			if (ModelState.IsValid)
			{
				if (model.Identifier.Length == 2)
				{
					var response = _localizationService.AddLanguage(model);
					if (response.IsSuccess)
					{
						return RedirectToAction("GetLanguages", "Localization");
					}

					foreach (var e in response.Errors)
					{
						ModelState.AddModelError(e.Key, e.Message);
					}

					return View(model);
				}

				ModelState.AddModelError(string.Empty, "Identifier must have only 2 characters");
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
			if (key != null)
			{
				var language =
					_locConfig.Value.Languages.Single(x =>
						string.Equals(x.Identifier, key, StringComparison.CurrentCultureIgnoreCase));
				return View(language);
			}

			return RedirectToAction("GetLanguages", "Localization", new {page = 1, perPage = 10});
		}

		/// <summary>
		/// Change status of language
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult ChangeStatus(LanguageCreateViewModel model)
		{
			var response = _localizationService.ChangeStatusOfLanguage(model);
			if (!response.IsSuccess)
			{
				foreach (var err in response.Errors)
				{
					ModelState.AddModelError(err.Key, err.Message);
				}

				return View(model);
			}

			return RedirectToAction("GetLanguages", "Localization", new {page = 1, perPage = 10});
		}
	}
}