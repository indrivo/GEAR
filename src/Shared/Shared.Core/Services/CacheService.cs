﻿using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Shared.Core.Services.Abstractions;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Core.Services
{
    public class CacheService : ICacheService
    {
        /// <summary>
        /// Inject distributed cache
        /// </summary>
        private readonly IDistributedCache _cache;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cache"></param>
        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// Set new value
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<bool> Set<TObject>(string key, TObject obj) where TObject : class
        {
            try
            {
                var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
                await _cache.SetAsync(key, bytes);
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Get value by key
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<TObject> Get<TObject>(string key) where TObject : class
        {
            try
            {
                var value = await _cache.GetAsync(key);
                if (value.Length == 0) return default;
                var str = Encoding.UTF8.GetString(value);
                return JsonConvert.DeserializeObject<TObject>(str);
            }
            catch
            {
                return default;
            }
        }
    }
}
