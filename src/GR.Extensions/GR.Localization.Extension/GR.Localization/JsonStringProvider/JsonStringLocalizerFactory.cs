using System;
using GR.Cache.Abstractions;
using GR.Localization.Abstractions;
using GR.Localization.Abstractions.Models.Config;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace GR.Localization.JsonStringProvider
{
    public class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        #region Injectable

        private readonly IHostingEnvironment _env;
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly IOptionsSnapshot<LocalizationConfig> _locConfig;
        private readonly ICacheService _cache;
        private readonly ILocalizationService _service;

        #endregion

        public JsonStringLocalizerFactory(IHttpContextAccessor httpAccessor,
            IHostingEnvironment env,
            IOptionsSnapshot<LocalizationConfig> locConfig,
            ILocalizationService service,
            ICacheService cache)
        {
            _env = env;
            _httpAccessor = httpAccessor;
            _locConfig = locConfig;
            _cache = cache;
            _service = service;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            return new JsonStringLocalizer(_env, _httpAccessor, _locConfig, _cache, _service);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new JsonStringLocalizer(_env, _httpAccessor, _locConfig, _cache, _service);
        }
    }
}
