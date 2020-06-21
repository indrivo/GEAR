using System;
using System.Threading.Tasks;
using GR.Cache.Abstractions;
using GR.Cache.Models;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;

namespace GR.Cache.Helpers
{
    internal class CacheServiceTemplate
    {
        protected ICacheService Container;

        public CacheServiceTemplate(ICacheService container)
        {
            Container = container;
        }

        /// <summary>
        /// Get or set request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<T>> GetOrSetResponseAsync<T>(string key, Func<Task<ResultModel<T>>> func)
            where T : class
        {
            var getData = await Container.GetAsync<T>(key);
            if (getData != null) return new SuccessResultModel<T>(getData);

            var request = await func();
            if (!request.IsSuccess) return request;
            await Container.SetAsync(key, request.Result);
            return request;
        }

        /// <summary>
        /// Get or set value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public virtual async Task<T> GetOrSetResponseAsync<T>(string key, Func<Task<T>> func)
            where T : class
        {
            var getData = await Container.GetAsync<T>(key);
            if (getData != null) return getData;

            var request = await func();
            await Container.SetAsync(key, request);
            return request;
        }

        /// <summary>
        /// Get or set in cache with expiration period
        /// Once time is left, the value is refreshed in cache
        /// This process is not automatic, is due on method call
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="life"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public virtual async Task<T> GetOrSetWithExpireTimeAsync<T>(string key, TimeSpan life, Func<Task<T>> func)
            where T : class
        {
            var getData = await Container.GetAsync<EntryLife<T>>(key);
            if (getData != null)
            {
                var valid = DateTime.Now - getData.DateTime < life;
                if (valid)
                    return getData.Data;
            }

            var request = await func();

            await Container.SetAsync(key, new EntryLife<T>
            {
                DateTime = DateTime.Now,
                Data = request
            });
            return request;
        }
    }
}