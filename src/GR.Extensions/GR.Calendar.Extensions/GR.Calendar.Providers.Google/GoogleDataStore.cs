using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util.Store;
using Mapster;
using GR.Calendar.Abstractions;
using GR.Calendar.Abstractions.Models;
using GR.Core.Extensions;

namespace GR.Calendar.Providers.Google
{
    public class GoogleDataStore : IDataStore
    {
        private readonly ICalendarExternalTokenProvider _tokenProvider;

        public GoogleDataStore(ICalendarExternalTokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        /// <inheritdoc />
        /// <summary>
        /// Clear 
        /// </summary>
        /// <returns></returns>
        public Task ClearAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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
                Value = value.SerializeAsJson()
            });
        }
    }
}
