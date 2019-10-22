using System;
using GR.Localization.Abstractions.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace GR.Localization
{
    public class JsonStringLocalizerFactory : IStringLocalizerFactory
	{
		private readonly IHostingEnvironment _env;
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly IOptionsSnapshot<LocalizationConfig> _locConfig;
        private readonly IDistributedCache _cache;

		public JsonStringLocalizerFactory(IHttpContextAccessor httpAccessor, 
            IHostingEnvironment env, 
            IOptionsSnapshot<LocalizationConfig> locConfig,
            IDistributedCache cache)
		{
			_env = env;
            _httpAccessor = httpAccessor;
            _locConfig = locConfig;
            _cache = cache;
		}

		public IStringLocalizer Create(Type resourceSource)
		{
			return new JsonStringLocalizer(_env, _httpAccessor, _locConfig, _cache);
		}

		public IStringLocalizer Create(string baseName, string location)
		{
			return new JsonStringLocalizer(_env, _httpAccessor, _locConfig, _cache);
		}
	}
}
