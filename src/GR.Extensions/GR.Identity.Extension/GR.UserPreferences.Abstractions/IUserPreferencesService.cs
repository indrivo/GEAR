using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.UserPreferences.Abstractions.Helpers.ResponseModels;

namespace GR.UserPreferences.Abstractions
{
    /// <summary>
    /// This abstraction is used for set and get user preferences
    /// </summary>
    public interface IUserPreferencesService
    {
        /// <summary>
        /// The value is provided for currently logged user
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<ResultModel<string>> GetValueByKeyAsync(string key);

        /// <summary>
        /// The value is provided for {userId} parameter
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<ResultModel<string>> GetValueByKeyAsync(Guid userId, string key);

        /// <summary>
        /// Get boolean value by key
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<ResultModel<bool>> GetBoolValueByKeyAsync(Guid userId, string key);

        /// <summary>
        /// Set value for some key on user preference
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<ResultModel> AddOrUpdatePreferenceSettingAsync(string key, string value);

        /// <summary>
        /// Get configuration of preference item with user selected option
        /// </summary>
        /// <returns></returns>
        Task<BaseBuildResponse<object>> GetPreferenceConfigurationAsync(string key);

        /// <summary>
        /// Get registered keys
        /// </summary>
        /// <returns></returns>
        ResultModel<IEnumerable<string>> GetAvailableKeys();
    }
}