using System;
using System.Threading.Tasks;
using ST.Core.Helpers;

namespace ST.Calendar.Abstractions
{
    public interface ICalendarUserSettingsService
    {
        /// <summary>
        /// Check if provider is enabled to sync events
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="providerName"></param>
        /// <returns></returns>
        Task<ResultModel> IsProviderEnabledAsync(Guid? userId, string providerName);

        /// <summary>
        /// Change user provider settings
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="provider"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        Task<ResultModel> ChangeProviderSettings(Guid? userId, string provider, bool enabled);

        /// <summary>
        /// Set event attribute
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="propName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<ResultModel> SetEventAttributeAsync(Guid? eventId, string propName, string value);

        /// <summary>
        /// Get event attribute
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        Task<ResultModel<string>> GetEventAttributeAsync(Guid? eventId, string propName);
    }
}