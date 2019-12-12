using System;
using System.Threading.Tasks;
using GR.Calendar.Abstractions.Models;
using GR.Core.Helpers;

namespace GR.Calendar.Abstractions
{
    public interface ICalendarExternalTokenProvider
    {
        /// <summary>
        /// Get token async
        /// </summary>
        /// <typeparam name="TTokenFormat"></typeparam>
        /// <param name="provider"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<ResultModel<TTokenFormat>> GetTokenAsync<TTokenFormat>(string provider, Guid? user) where TTokenFormat : class;

        /// <summary>
        /// Set toke async
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<ResultModel> SetTokenAsync(ExternalProviderToken token);

        /// <summary>
        /// Delete token
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<ResultModel> DeleteTokenAsync(string provider, Guid? user);
    }
}