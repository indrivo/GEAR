using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ST.BaseBusinessRepository;
using ST.Localization.Razor.Services.Abstractions;
using ST.Localization.Razor.ViewModels.LocalizationViewModels;
using YandexTranslateCSharpSdk;

namespace ST.Localization.Razor.Services
{
    public class LocalizationService : ILocalizationService
    {
        private readonly IOptionsSnapshot<LocalizationConfigModel> _locConfig;
        private readonly IHostingEnvironment _env;
        private readonly IStringLocalizer _localizer;
        private readonly IDistributedCache _cache;
        private readonly YandexTranslateSdk _yandexTranslateSdk;

        public LocalizationService(IOptionsSnapshot<LocalizationConfigModel> locConfig, IHostingEnvironment env,
            IStringLocalizer localizer, IDistributedCache cache)
        {
            _env = env;
            _locConfig = locConfig;
            _localizer = localizer;
            _cache = cache;
            _yandexTranslateSdk = new YandexTranslateSdk();
            _yandexTranslateSdk.ApiKey = "trnsl.1.1.20190428T193614Z.41a78cdc7c5bb298.e25c6bd03beee1462ab0d452f9d2a86c1fc6013f";
        }

        /// <summary>
        /// Add language
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel AddLanguage(AddLanguageViewModel model)
        {
            var response = new ResultModel { IsSuccess = true };
            var existsInConfig =
                _locConfig.Value.Languages.Any(m => m.Identifier == model.Identifier && m.Name == model.Name);

            var cPaths = new[]
            {
                _env.ContentRootPath,
                _locConfig.Value.Path,
                string.Format("{0}.json", model.Identifier)
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
                    var keys = _localizer.GetAllForLanguage("en").ToList();
                    var dict = new Dictionary<string, string>();
                    foreach (var item in keys)
                    {
                        var translated = _yandexTranslateSdk
                            .TranslateText(item.Value, $"en-{model.Identifier}")
                            .GetAwaiter()
                            .GetResult();
                        dict.Add(item.Name, translated);
                    }
                    var obj = JObject.FromObject(dict);

                    writer.Formatting = Formatting.Indented;
                    obj.WriteTo(writer);
                }

                var langsFile = _env.ContentRootFileProvider.GetFileInfo("appsettings.json");

                using (Stream str = new FileStream(langsFile.PhysicalPath, FileMode.Open, FileAccess.Read,
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
                    var newLangs = JArray.FromObject(_locConfig.Value.Languages);
                    fileObj[nameof(LocalizationConfig)][nameof(LocalizationConfig.Languages)] = newLangs;
                    reader.Close();
                    File.WriteAllText(langsFile.PhysicalPath, fileObj.ToString());
                }
            }

            return response;
        }

        /// <summary>
        /// Change statu of language
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
                string.Format("{0}.json", model.Identifier)
            };

            var filePath = Path.Combine(cPaths);
            var fileExists = File.Exists(filePath);

            if (existsInConfig || fileExists)
            {
                var langsFile = _env.ContentRootFileProvider.GetFileInfo("appsettings.json");
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
                                IsDisabled = false
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
        /// Edit key
        /// </summary>
        /// <param name="model"></param>
        public void EditKey(EditLocalizationViewModel model)
        {
            var newStrings = model.LocalizedStrings;
            AddOrUpdateKey(model.Key, newStrings);
        }

        /// <summary>
        /// Update keys
        /// </summary>
        /// <param name="key"></param>
        /// <param name="localizedStrings"></param>
        public void AddOrUpdateKey(string key, IDictionary<string, string> localizedStrings)
        {
            foreach (var item in localizedStrings)
            {
                var filePath = Path.Combine(_env.ContentRootPath, _locConfig.Value.Path, item.Key + ".json");
                var cacheKey = $"{_locConfig.Value.SessionStoreKeyName}_{item.Key}_{key}";
                _cache.SetString(cacheKey, item.Value);
                if (!File.Exists(filePath))
                {
                    using (Stream str = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write,
                        FileShare.Write))
                    using (var sWriter = new StreamWriter(str))
                    using (var writer = new JsonTextWriter(sWriter))
                    {
                        writer.Formatting = Formatting.Indented;
                        writer.WriteStartObject();
                        writer.WritePropertyName(key);
                        writer.WriteValue(item.Value);
                        writer.WriteEndObject();
                    }
                }
                else
                {
                    using (Stream stream =
                        new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var sReader = new StreamReader(stream))
                    using (var reader = new JsonTextReader(sReader))
                    {
                        var jobj = JObject.Load(reader);
                        jobj[key] = item.Value;
                        reader.Close();
                        File.WriteAllText(filePath, jobj.ToString());
                    }
                }
            }
        }
    }
}