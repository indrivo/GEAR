using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GR.Cache.Abstractions;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Helpers.Responses;
using GR.Core.Helpers.Validators;
using GR.Localization.Abstractions;
using GR.Localization.Abstractions.Models;
using GR.Localization.Abstractions.Models.Config;
using GR.Localization.Abstractions.ViewModels.LocalizationViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GR.Localization.JsonStringProvider
{
    [Author(Authors.LUPEI_NICOLAE, 1.2, "Add new features, change all methods Task based")]
    public class JsonFileLocalizationService : ILocalizationService
    {
        #region Injectable

        private readonly IOptionsSnapshot<LocalizationConfigModel> _locConfig;
        private readonly IHostingEnvironment _env;
        private readonly ICacheService _cache;
        private readonly IExternalTranslationProvider _externalTranslationProvider;
        private readonly IMapper _mapper;

        #endregion

        public JsonFileLocalizationService(IOptionsSnapshot<LocalizationConfigModel> locConfig, IHostingEnvironment env, ICacheService cache, IExternalTranslationProvider externalTranslationProvider, IMapper mapper)
        {
            _env = env;
            _locConfig = locConfig;
            _cache = cache;
            _externalTranslationProvider = externalTranslationProvider;
            _mapper = mapper;
        }

        /// <summary>
        /// Add language
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResultModel> AddLanguageAsync(AddLanguageViewModel model)
        {
            var isValid = ModelValidator.IsValid(model);
            if (!isValid.IsSuccess) return isValid.ToBase();
            var response = new ResultModel { IsSuccess = true };
            var existsInConfig =
                _locConfig.Value.Languages.Any(m => m.Identifier == model.Identifier && m.Name == model.Name);

            var cPaths = new[]
            {
                _env.ContentRootPath,
                _locConfig.Value.Path,
                $"{model.Identifier}.json"
            };

            var filePath = Path.Combine(cPaths);
            var fileExists = File.Exists(filePath);

            if (existsInConfig || fileExists)
            {
                response.Errors.Add(new ErrorModel
                {
                    Key = string.Empty,
                    Message = "Language already exists."
                });
                response.IsSuccess = false;
            }
            else
            {
                using (Stream stream =
                    new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
                using (var sWriter = new StreamWriter(stream))
                using (var writer = new JsonTextWriter(sWriter))
                {
                    var pack = await GetLanguagePackAsync("en");
                    if (!pack.IsSuccess) return pack.ToBase();
                    var keys = pack.Result.Select(x => new LocalizedString(x.Key, x.Value));
                    var dict = new Dictionary<string, string>();
                    foreach (var item in keys)
                    {
                        var translated = _externalTranslationProvider.TranslateText(item.Value, "en", model.Identifier);
                        dict.Add(item.Name, translated);
                    }
                    var obj = JObject.FromObject(dict);

                    writer.Formatting = Formatting.Indented;
                    obj.WriteTo(writer);
                }

                var languagesFile = _env.ContentRootFileProvider.GetFileInfo(ResourceProvider.AppSettingsFilepath(_env));

                using (Stream str = new FileStream(languagesFile.PhysicalPath, FileMode.Open, FileAccess.Read,
                    FileShare.ReadWrite))
                using (var sReader = new StreamReader(str))
                using (var reader = new JsonTextReader(sReader))
                {
                    var fileObj = JObject.Load(reader);
                    _locConfig.Value.Languages.Add(new LanguageCreateViewModel
                    {
                        Identifier = model.Identifier,
                        Name = model.Name,
                        IsDisabled = false
                    });
                    var newLanguages = JArray.FromObject(_locConfig.Value.Languages);
                    fileObj[nameof(LocalizationConfig)][nameof(LocalizationConfig.Languages)] = newLanguages;
                    reader.Close();
                    try
                    {
                        await File.WriteAllTextAsync(languagesFile.PhysicalPath, fileObj.ToString());
                    }
                    catch (Exception e)
                    {
                        response.IsSuccess = false;
                        response.AddError(e.Message);
                    }
                }
            }

            return response;
        }

        /// <summary>
        /// Change status of language
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel ChangeStatusOfLanguage(LanguageCreateViewModel model)
        {
            var response = new ResultModel { IsSuccess = true };
            var existsInConfig =
                _locConfig.Value.Languages.Any(m => m.Identifier == model.Identifier && m.Name == model.Name);

            var cPaths = new[]
            {
                _env.ContentRootPath,
                _locConfig.Value.Path,
                $"{model.Identifier}.json"
            };

            var filePath = Path.Combine(cPaths);
            var fileExists = File.Exists(filePath);

            if (existsInConfig || fileExists)
            {
                var langsFile = _env.ContentRootFileProvider.GetFileInfo(ResourceProvider.AppSettingsFilepath(_env));
                using (Stream str = new FileStream(langsFile.PhysicalPath, FileMode.Open, FileAccess.Read,
                    FileShare.ReadWrite))
                using (var sReader = new StreamReader(str))
                using (var reader = new JsonTextReader(sReader))
                {
                    var fileObj = JObject.Load(reader);
                    var languages = _locConfig.Value.Languages;
                    var updLangs = new HashSet<LanguageCreateViewModel>();
                    foreach (var e in languages)
                    {
                        if (e.Identifier == model.Identifier)
                        {
                            updLangs.Add(new LanguageCreateViewModel
                            {
                                Identifier = model.Identifier,
                                Name = model.Name,
                                IsDisabled = model.IsDisabled
                            });
                        }
                        else
                        {
                            updLangs.Add(new LanguageCreateViewModel
                            {
                                Identifier = e.Identifier,
                                Name = e.Name,
                                IsDisabled = e.IsDisabled
                            });
                        }
                    }

                    var updatedLangs = JObject
                        .FromObject(new LocalizationConfigModel
                        {
                            DefaultLanguage = _locConfig.Value.DefaultLanguage,
                            Languages = updLangs,
                            Path = _locConfig.Value.Path,
                            SessionStoreKeyName = _locConfig.Value.SessionStoreKeyName
                        });

                    fileObj[nameof(LocalizationConfig)] = updatedLangs;
                    reader.Close();
                    File.WriteAllText(langsFile.PhysicalPath, fileObj.ToString());
                }
            }
            else
            {
                response.IsSuccess = false;
                response.Errors.Add(new ErrorModel
                {
                    Key = string.Empty,
                    Message = "Language does exists"
                });
            }

            return response;
        }

        /// <summary>
        /// Import language translations
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="translations"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> ImportLanguageTranslationsAsync(string identifier, Dictionary<string, string> translations)
        {
            var response = new ResultModel();
            var languageRequest = await GetLanguageByIdentifierAsync(identifier);
            if (!languageRequest.IsSuccess) return languageRequest.ToBase();
            var filePath = Path.Combine(_env.ContentRootPath, _locConfig.Value.Path, identifier + ".json");
            foreach (var (key, value) in translations)
            {
                var cacheKey = GenerateKey(identifier, key);
                await _cache.SetAsync(cacheKey, value);
            }

            var obj = JObject.Parse(translations.SerializeAsJson());
            try
            {
                await File.WriteAllTextAsync(filePath, obj.ToString(Formatting.Indented));
                response.IsSuccess = true;
            }
            catch (Exception e)
            {
                response.AddError(e.Message);
            }

            return response;
        }

        /// <summary>
        /// Get all languages
        /// </summary>
        /// <returns></returns>
        public virtual Task<ResultModel<IEnumerable<LanguageCreateViewModel>>> GetAllLanguagesAsync()
        {
            var languages = _locConfig.Value.Languages
                .Where(x => !x.IsDisabled).ToList();
            ResultModel<IEnumerable<LanguageCreateViewModel>> response =
                new SuccessResultModel<IEnumerable<LanguageCreateViewModel>>(languages);
            return Task.FromResult(response);
        }

        /// <summary>
        /// Get language by identifier
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public Task<ResultModel<Language>> GetLanguageByIdentifierAsync(string identifier)
        {
            var language = _locConfig.Value.Languages.FirstOrDefault(x => x.Identifier.Equals(identifier));
            if (language == null) return Task.FromResult<ResultModel<Language>>(new NotFoundResultModel<Language>());
            var mapped = _mapper.Map<Language>(language);
            return Task.FromResult<ResultModel<Language>>(new SuccessResultModel<Language>(mapped));
        }

        /// <summary>
        /// Generate key
        /// </summary>
        /// <param name="language"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual string GenerateKey(string language, string key)
        {
            var cacheKey = $"{_locConfig.Value.SessionStoreKeyName}_{language}_{key}";
            return cacheKey;
        }

        /// <summary>
        /// Get language pack
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Dictionary<string, string>>> GetLanguagePackAsync(string language)
        {
            var filePath = Path.Combine(_env.ContentRootPath, _locConfig.Value.Path, language + ".json");
            if (!File.Exists(filePath))
                return new NotFoundResultModel<Dictionary<string, string>>();
            var text = await File.ReadAllTextAsync(filePath);
            var dict = text.Deserialize<Dictionary<string, string>>();
            return new SuccessResultModel<Dictionary<string, string>>(dict);
        }

        /// <summary>
        /// Get languages packs
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
        /// Edit key
        /// </summary>
        /// <param name="model"></param>
        public async Task<ResultModel> EditKeyAsync([Required]EditLocalizationViewModel model)
        {
            var modelState = ModelValidator.IsValid(model);
            if (!modelState.IsSuccess) return modelState;
            var newStrings = model.LocalizedStrings;
            return await AddOrUpdateKeyAsync(model.Key, newStrings);
        }

        /// <summary>
        /// Update keys
        /// </summary>
        /// <param name="key"></param>
        /// <param name="localizedStrings"></param>
        public async Task<ResultModel> AddOrUpdateKeyAsync(string key, IDictionary<string, string> localizedStrings)
        {
            var response = new ResultModel();
            if (key.IsNullOrEmpty() || localizedStrings == null) return new InvalidParametersResultModel();
            foreach (var (s, value) in localizedStrings)
            {
                var filePath = Path.Combine(_env.ContentRootPath, _locConfig.Value.Path, s + ".json");
                var cacheKey = GenerateKey(s, key);
                await _cache.SetAsync(cacheKey, value);
                if (!File.Exists(filePath))
                {
                    using (Stream str = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write,
                        FileShare.Write))
                    using (var sWriter = new StreamWriter(str))
                    using (var writer = new JsonTextWriter(sWriter))
                    {
                        writer.Formatting = Formatting.Indented;
                        await writer.WriteStartObjectAsync();
                        await writer.WritePropertyNameAsync(key);
                        await writer.WriteValueAsync(value);
                        await writer.WriteEndObjectAsync();
                    }

                    response.IsSuccess = true;
                }
                else
                {
                    using (Stream stream =
                        new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var sReader = new StreamReader(stream))
                    using (var reader = new JsonTextReader(sReader))
                    {
                        var obj = JObject.Load(reader);
                        obj[key] = value;
                        reader.Close();
                        try
                        {
                            await File.WriteAllTextAsync(filePath, obj.ToString());
                            response.IsSuccess = true;
                        }
                        catch (Exception e)
                        {
                            response.AddError(e.Message);
                        }
                    }
                }
            }
            return response;
        }
    }
}