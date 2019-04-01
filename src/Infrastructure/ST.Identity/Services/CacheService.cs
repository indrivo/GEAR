using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using ST.Identity.Services.Abstractions;

namespace ST.Identity.Services
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
        public virtual async Task<bool> Set<TObject>(string key, TObject obj) where TObject : class, ICacheModel
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
        public virtual async Task<TObject> Get<TObject>(string key) where TObject : class, ICacheModel
        {
            try
            {
                var value = await _cache.GetAsync(key);
                if (value == null || value.Length == 0) return default;
                var str = Encoding.UTF8.GetString(value);
                var data = JsonConvert.DeserializeObject<TObject>(str);
                data.IsSuccess = true;
                return data;
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// Remove key from cache service
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual async Task RemoveAsync(string key) => await _cache.RemoveAsync(key);
    }
}
