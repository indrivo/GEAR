using System;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util.Store;
using Mapster;
using ST.Calendar.Abstractions;
using ST.Calendar.Abstractions.Models;
using ST.Core.Extensions;

namespace ST.Calendar.Providers.Google
{
    public class GoogleDataStore : IDataStore
    {
        private readonly ICalendarExternalTokenProvider _tokenProvider;

        public GoogleDataStore(ICalendarExternalTokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        /// <summary>
        /// Clear 
        /// </summary>
        /// <returns></returns>
        public Task ClearAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Delete key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task DeleteAsync<T>(string key)
        {
            await _tokenProvider.DeleteTokenAsync(nameof(GoogleCalendarProvider), key.ToGuid());
        }

        /// <summary>
        /// Get token
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string key)
        {
            var tokenRequest = await _tokenProvider.GetTokenAsync<TokenResponse>(nameof(GoogleCalendarProvider), key.ToGuid());
            return tokenRequest.IsSuccess ? tokenRequest.Result.Adapt<T>() : default;
        }

        /// <summary>
        /// Store token
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task StoreAsync<T>(string key, T value)
        {
            await _tokenProvider.SetTokenAsync(new ExternalProviderToken
            {
                UserId = key.ToGuid(),
                ProviderName = nameof(GoogleCalendarProvider),
                Value = value.Serialize()
            });
        }
    }
}
