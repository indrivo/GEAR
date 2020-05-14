using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GR.Cache.Abstractions;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.Core.Helpers.Validators;
using GR.Localization.Abstractions;
using GR.Localization.Abstractions.Models;
using GR.Localization.Abstractions.ViewModels.LocalizationViewModels;
using Microsoft.EntityFrameworkCore;

namespace GR.Localization.DataBaseProvider
{
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

        #endregion

        public DbLocalizationService(ILocalizationContext context, ICacheService cacheService, IMapper mapper)
        {
            _context = context;
            _cacheService = cacheService;
            _mapper = mapper;
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
        /// Edit key
        /// </summary>
        /// <param name="model"></param>
        public virtual void EditKey(EditLocalizationViewModel model)
        {
            var newStrings = model.LocalizedStrings;
            AddOrUpdateKey(model.Key, newStrings);
        }

        public virtual void AddOrUpdateKey(string key, IDictionary<string, string> localizedStrings)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add language
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual ResultModel AddLanguage(AddLanguageViewModel model)
        {
            if (model.IsNull()) return new InvalidParametersResultModel();
            var isValid = ModelValidator.IsValid(model);
            if (!isValid.IsSuccess) return isValid.ToBase();
            if (_context.Languages.Any(x => x.Identifier.Equals(model.Identifier)))
                return new InvalidParametersResultModel("language already exist");
            _context.Languages.Add(new Language
            {
                Name = model.Name,
                Identifier = model.Identifier
            });
            return _context.Push();
        }

        public virtual ResultModel ChangeStatusOfLanguage(LanguageCreateViewModel model)
        {
            throw new NotImplementedException();
        }
    }
}
