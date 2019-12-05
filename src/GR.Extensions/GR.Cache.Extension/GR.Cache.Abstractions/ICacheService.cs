using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace GR.Cache.Abstractions
{
    public interface ICacheService
    {
        Task<bool> Set<TObject>(string key, TObject obj) where TObject : class;
        Task<TObject> Get<TObject>(string key) where TObject : class;
        Task RemoveAsync(string key);
        IEnumerable<RedisKey> GetAllKeys();
        IEnumerable<RedisKey> GetAllByPatternFilter(string pattern);
        bool IsConnected();
        void FlushAll();
    }
}
