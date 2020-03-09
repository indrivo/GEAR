using System;
using GR.Cache.Abstractions;
using GR.Localization.Abstractions;
using GR.Localization.Abstractions.Models.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace GR.Localization.DataBaseProvider
{
    public class DbLocalizerFactory : IStringLocalizerFactory
    {
        #region Injectable

        private readonly ILocalizationContext _localizationContext;
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly IOptionsSnapshot<LocalizationConfig> _locConfig;
        private readonly ICacheService _cache;

        #endregion

        public DbLocalizerFactory(IHttpContextAccessor httpAccessor,
            ILocalizationContext localizationContext,
            IOptionsSnapshot<LocalizationConfig> locConfig,
            ICacheService cache)
        {
            _httpAccessor = httpAccessor;
            _localizationContext = localizationContext;
            _locConfig = locConfig;
            _cache = cache;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            return new DbStringLocalizer(_localizationContext, _cache, _httpAccessor, _locConfig);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new DbStringLocalizer(_localizationContext, _cache, _httpAccessor, _locConfig);
        }
    }
}
