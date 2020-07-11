﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GR.Cache.Abstractions;
using GR.Core;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Helpers.Responses;
using GR.Core.Helpers.Validators;
using GR.Localization.Abstractions;
using GR.Localization.Abstractions.Extensions;
using GR.Localization.Abstractions.Models;
using GR.Localization.Abstractions.ViewModels.LocalizationViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace GR.Localization.DataBaseProvider
{
    /// <summary>
    /// This is an implementation of localization service with EF
    /// </summary>
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    public class DbLocalizationService : ILocalizationService
    {
        #region Injectable

        /// <summary>
        /// Inject context
        /// </summary>
        private readonly ILocalizationContext _context;

        /// <summary>
        /// Inject cache service
        /// </summary>
        private readonly ICacheService _cacheService;

        /// <summary>
        /// Inject mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Inject context accessor
        /// </summary>
        private readonly IHttpContextAccessor _contextAccessor;

        /// <summary>
        /// Inject options
        /// </summary>
        private readonly IOptionsSnapshot<LocalizationConfigModel> _options;

        #endregion

        public DbLocalizationService(ILocalizationContext context, ICacheService cacheService, IMapper mapper, IHttpContextAccessor contextAccessor, IOptionsSnapshot<LocalizationConfigModel> options)
        {
            _context = context;
            _cacheService = cacheService;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _options = options;
        }

        /// <summary>
        /// Import language translations
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="translations"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> ImportLanguageTranslationsAsync(string identifier, Dictionary<string, string> translations)
        {
            var languageRequest = await GetLanguageByIdentifierAsync(identifier);
            if (!languageRequest.IsSuccess) return languageRequest.ToBase();
            foreach (var (key, value) in translations)
            {
                var keyEntity = await _context.Translations.FirstOrDefaultAsync(x => x.Key.Equals(key));
                if (keyEntity == null)
                {
                    keyEntity = new Translation
                    {
                        Key = key
                    };
                    await _context.Translations.AddAsync(keyEntity);
                    await _context.PushAsync();
                }

                var translationItem = await _context.TranslationItems
                    .FirstOrDefaultAsync(x => x.Identifier.Equals(identifier) && x.TranslationId.Equals(keyEntity.Id));
                if (translationItem == null)
                {
                    await _context.TranslationItems.AddAsync(new TranslationItem
                    {
                        Identifier = identifier,
                        TranslationId = keyEntity.Id,
                        Value = value
                    });
                }
                else
                {
                    translationItem.Value = value;
                    _context.TranslationItems.Update(translationItem);
                }
            }

            return await _context.PushAsync();
        }

        /// <summary>
        /// Get all languages
        /// </summary>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<LanguageCreateViewModel>>> GetAllLanguagesAsync()
        {
            var languages = await _context.Languages.ToListAsync();
            var mapped = _mapper.Map<IEnumerable<LanguageCreateViewModel>>(languages);
            return new SuccessResultModel<IEnumerable<LanguageCreateViewModel>>(mapped);
        }

        /// <summary>
        /// Get language by id
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Language>> GetLanguageByIdentifierAsync(string identifier)
        {
            if (identifier.IsNullOrEmpty()) return new InvalidParametersResultModel<Language>();
            var language = await _context.Languages.FirstOrDefaultAsync(x => x.Identifier.Equals(identifier));
            if (language == null) return new NotFoundResultModel<Language>();
            return new SuccessResultModel<Language>(language);
        }

        /// <summary>
        /// Generate key
        /// </summary>
        /// <param name="language"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GenerateKey(string language, string key)
        {
            return $"_lang_key_{language}_{key}";
        }

        /// <summary>
        /// Get language pack
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Dictionary<string, string>>> GetLanguagePackAsync(string language)
        {
            var items = await _context.TranslationItems
                .Include(x => x.Translation)
                .AsNoTracking()
                .Where(x => x.Identifier.Equals(language))
                .ToListAsync();

            var dict = items.ToDictionary(item => item.Translation.Key, item => item.Value);
            return new SuccessResultModel<Dictionary<string, string>>(dict);
        }

        /// <summary>
        /// Get language packs
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<Dictionary<string, Dictionary<string, string>>>> GetLanguagePacksAsync()
        {
            var languagesReq = await GetAllLanguagesAsync();
            if (!languagesReq.IsSuccess) return languagesReq.Map<Dictionary<string, Dictionary<string, string>>>();
            var dict = new Dictionary<string, Dictionary<string, string>>();
            foreach (var lang in languagesReq.Result.ToList())
            {
                var packRequest = await GetLanguagePackAsync(lang.Identifier);
                if (packRequest.IsSuccess) dict.Add(lang.Identifier, packRequest.Result);
            }
            return new SuccessResultModel<Dictionary<string, Dictionary<string, string>>>(dict);
        }

        /// <summary>
        /// Get current language
        /// </summary>
        /// <returns></returns>
        public virtual async Task<Language> GetCurrentLanguageAsync()
        {
            var languageId = _contextAccessor.HttpContext.Session.GetString(_options.Value.SessionStoreKeyName);
            return (await GetLanguageByIdentifierAsync(languageId)).Result;
        }

        /// <summary>
        /// Edit key
        /// </summary>
        /// <param name="model"></param>
        public virtual async Task<ResultModel> EditKeyAsync(EditLocalizationViewModel model)
        {
            var validate = ModelValidator.IsValid(model);
            if (!validate.IsSuccess) return validate;
            var newStrings = model.LocalizedStrings;
            return await AddOrUpdateKeyAsync(model.Key, newStrings);
        }

        /// <summary>
        /// Add or update key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="localizedStrings"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> AddOrUpdateKeyAsync(string key, IDictionary<string, string> localizedStrings)
        {
            if (key.IsNullOrEmpty() || localizedStrings == null) return new InvalidParametersResultModel();
            var dbKey = await _context.Translations
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Key.Equals(key));
            if (dbKey == null)
            {
                dbKey = new Translation
                {
                    Key = key
                };
                await _context.Translations.AddAsync(dbKey);
                var dbRequest = await _context.PushAsync();
                if (!dbRequest.IsSuccess)
                    return dbRequest;
            }
            foreach (var (s, value) in localizedStrings)
            {
                var translationItem = await _context.TranslationItems
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Identifier.Equals(s)
                                              && x.TranslationId.Equals(dbKey.Id));
                if (translationItem == null)
                {
                    await _context.TranslationItems.AddAsync(new TranslationItem
                    {
                        Identifier = s,
                        TranslationId = dbKey.Id,
                        Value = value
                    });
                }
                else
                {
                    translationItem.Value = value;
                    _context.TranslationItems.Update(translationItem);
                    var cacheKey = GenerateKey(s, key);
                    await _cacheService.SetAsync(cacheKey, value);
                }
            }

            return await _context.PushAsync();
        }

        /// <summary>
        /// Add language
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> AddLanguageAsync(AddLanguageViewModel model)
        {
            if (model.IsNull()) return new InvalidParametersResultModel();
            var isValid = ModelValidator.IsValid(model);
            if (!isValid.IsSuccess) return isValid.ToBase();
            if (await _context.Languages.AnyAsync(x => x.Identifier.Equals(model.Identifier)))
                return new InvalidParametersResultModel("language already exist");
            await _context.Languages.AddAsync(new Language
            {
                Name = model.Name,
                Identifier = model.Identifier
            });
            return await _context.PushAsync();
        }

        /// <summary>
        /// Change status of language
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> ChangeStatusOfLanguageAsync(LanguageCreateViewModel model)
        {
            var languageRequest = await GetLanguageByIdentifierAsync(model.Identifier);
            if (!languageRequest.IsSuccess) return languageRequest.ToBase();
            var language = languageRequest.Result;
            language.IsDeleted = model.IsDisabled;
            return await _context.PushAsync();
        }

        /// <summary>
        /// Get languages with pagination
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual async Task<DTResult<LanguageCreateViewModel>> GetLanguagesWithPaginationAsync(DTParameters parameters)
        {
            var paginated = await _context.Languages.GetPagedAsDtResultAsync(parameters);
            return _mapper.Map<DTResult<LanguageCreateViewModel>>(paginated);
        }

        /// <summary>
        /// Get keys with pagination
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual async Task<DTResult<LocalizedString>> GetLocalizationKeysWithPaginationAsync(DTParameters parameters)
        {
            var stringLocalizer = IoC.Resolve<IStringLocalizer>();
            var paginated = await stringLocalizer.GetAllStrings()
                .AsAsyncQueryable()
                .GetPagedAsDtResultAsync(parameters);
            return paginated;
        }

        /// <summary>
        /// Get key configuration
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual ResultModel<EditLocalizationViewModel> GetKeyConfiguration(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return new NotFoundResultModel<EditLocalizationViewModel>();
            }

            var stringLocalizer = IoC.Resolve<IStringLocalizer>();

            var rz = new Dictionary<string, string>();
            foreach (var item in _options.Value.Languages)
            {
                var str = stringLocalizer.GetForLanguage(key, item.Identifier);
                rz.Add(item.Identifier, str.Value != $"[{str.Name}]" ? str : string.Empty);
            }

            var model = new EditLocalizationViewModel
            {
                Key = key,
                LocalizedStrings = rz,
                Languages = _options.Value.Languages.ToDictionary(f => f.Identifier, f => f.Name)
            };
            return new SuccessResultModel<EditLocalizationViewModel>(model);
        }

        /// <summary>
        /// Get add key configuration
        /// </summary>
        /// <returns></returns>
        public virtual ResultModel<AddKeyViewModel> GetAddKeyConfiguration()
        {
            var rz = _options.Value.Languages.ToDictionary(item => item.Identifier, item => string.Empty);

            var model = new AddKeyViewModel
            {
                LocalizedStrings = rz,
                Languages = _options.Value.Languages.ToDictionary(f => f.Identifier, f => f.Name)
            };
            return new SuccessResultModel<AddKeyViewModel>(model);
        }
    }
}