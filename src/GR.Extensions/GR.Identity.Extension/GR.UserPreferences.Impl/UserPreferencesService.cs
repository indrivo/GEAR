using GR.UserPreferences.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Helpers.Responses;
using GR.Identity.Abstractions;
using GR.UserPreferences.Abstractions.Helpers;
using GR.UserPreferences.Abstractions.Helpers.ResponseModels;
using GR.UserPreferences.Abstractions.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace GR.UserPreferences.Impl
{
    /// <summary>
    /// This is a implementation of user preference service with EF and Identity,
    /// it use memory cache
    /// </summary>
    [Author(Authors.LUPEI_NICOLAE)]
    public class UserPreferencesService : IUserPreferencesService
    {
        #region Injectable

        /// <summary>
        /// Inject context
        /// </summary>
        private readonly IUserPreferencesContext _context;

        /// <summary>
        /// Inject cache service
        /// </summary>
        private readonly IMemoryCache _cache;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        /// <summary>
        /// Inject preferences provider
        /// </summary>
        private readonly DefaultUserPreferenceProvider _provider;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cache"></param>
        /// <param name="userManager"></param>
        /// <param name="provider"></param>
        public UserPreferencesService(IUserPreferencesContext context, IMemoryCache cache, IUserManager<GearUser> userManager, DefaultUserPreferenceProvider provider)
        {
            _context = context;
            _cache = cache;
            _userManager = userManager;
            _provider = provider;
        }

        /// <summary>
        /// Get value by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<string>> GetValueByKeyAsync(string key)
        {
            if (key.IsNullOrEmpty()) return new InvalidParametersResultModel<string>();
            var userIdReq = _userManager.FindUserIdInClaims();
            if (!userIdReq.IsSuccess) return userIdReq.Map<string>();
            var userId = userIdReq.Result;
            var cacheKey = BuildKey(userId, key);
            var cacheGet = _cache.Get<UserPreference>(cacheKey);
            if (cacheGet != null) return new SuccessResultModel<string>(cacheGet.Value);
            var item = await _context.UserPreferences
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId.Equals(userId) && x.Key.Equals(key));
            if (item == null) return new NotFoundResultModel<string>();
            _cache.Set(cacheKey, item);
            return new SuccessResultModel<string>(item.Value);
        }

        /// <summary>
        /// Add or update user preference
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> AddOrUpdatePreferenceSettingAsync(string key, string value)
        {
            if (key.IsNullOrEmpty()) return new InvalidParametersResultModel();
            var userIdReq = _userManager.FindUserIdInClaims();
            if (!userIdReq.IsSuccess) return userIdReq.ToBase();
            var userId = userIdReq.Result;
            if (!_provider.Exist(key)) return new InvalidParametersResultModel("No registered key");
            var modelState = _provider.ValidateValueByKey(key, value);
            if (!modelState.IsSuccess) return modelState;
            var cacheKey = BuildKey(userId, key);
            var item = await _context.UserPreferences
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId.Equals(userId) && x.Key.Equals(key));
            if (item == null)
            {
                await _context.UserPreferences.AddAsync(new UserPreference
                {
                    Key = key,
                    UserId = userId,
                    Value = value
                });
            }
            else
            {
                item.Value = value;
                _context.UserPreferences.Update(item);
            }

            var dbResponse = await _context.PushAsync();
            if (dbResponse.IsSuccess)
            {
                _cache.Set(cacheKey, item);
            }

            return dbResponse;
        }

        /// <summary>
        /// Get configuration for key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual async Task<BaseBuildResponse<object>> GetPreferenceConfigurationAsync(string key)
        {
            var response = new BaseBuildResponse<object>();
            if (key.IsNullOrEmpty())
            {
                response.AddError("Key is required");
                return response;
            }
            var valueRequest = await GetValueByKeyAsync(key);
            var confRequest = await _provider.GetConfigurationAsync(key, valueRequest.Result);
            return confRequest;
        }

        /// <summary>
        /// Get available keys
        /// </summary>
        /// <returns></returns>
        public virtual ResultModel<IEnumerable<string>> GetAvailableKeys()
            => new SuccessResultModel<IEnumerable<string>>(_provider.GetAvailableKeys());

        #region Helpers

        /// <summary>
        /// Build cache preference key 
        /// </summary>
        /// <param name="user">User id</param>
        /// <param name="key">Key of preference configuration</param>
        /// <returns></returns>
        private static string BuildKey(Guid user, string key)
            => $"preference_key_{user}_{key}";

        #endregion
    }
}