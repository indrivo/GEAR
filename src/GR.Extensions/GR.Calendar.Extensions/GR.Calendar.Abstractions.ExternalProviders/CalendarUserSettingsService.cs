using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GR.Calendar.Abstractions.Models;
using GR.Core.Extensions;
using GR.Core.Helpers;

namespace GR.Calendar.Abstractions.ExternalProviders
{
    public class CalendarUserSettingsService : ICalendarUserSettingsService
    {
        #region Injectable

        /// <summary>
        /// Inject db context
        /// </summary>
        private readonly ICalendarDbContext _context;

        #endregion

        public CalendarUserSettingsService(ICalendarDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        /// <summary>
        /// Check if provider is enabled
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> IsProviderEnabledAsync(Guid? userId, string providerName)
        {
            var response = new ResultModel();
            if (!userId.HasValue)
            {
                response.Errors.Add(new ErrorModel(string.Empty, "Invalid parameter"));
                return response;
            }

            var settings = await _context.UserProviderSyncPreferences
                .FirstOrDefaultAsync(x => x.UserId.Equals(userId) && x.Provider.Equals(providerName));

            if (settings != null && settings.Sync) response.IsSuccess = true;
            return response;
        }

        /// <inheritdoc />
        /// <summary>
        /// Change provider settings
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="provider"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> ChangeProviderSettings(Guid? userId, string provider, bool enabled)
        {
            var response = new ResultModel();
            if (!userId.HasValue)
            {
                response.Errors.Add(new ErrorModel(string.Empty, "Invalid parameter"));
                return response;
            }

            var settings = await _context.UserProviderSyncPreferences
                .FirstOrDefaultAsync(x => x.UserId.Equals(userId) && x.Provider.Equals(provider));
            if (settings != null)
            {
                settings.Sync = enabled;
                _context.Update(settings);
            }
            else
            {
                await _context.UserProviderSyncPreferences.AddAsync(new UserProviderSyncPreference
                {
                    UserId = userId.Value,
                    Sync = enabled,
                    Provider = provider
                });
            }

            return await _context.PushAsync();
        }

        /// <inheritdoc />
        /// <summary>
        /// Set event attribute
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="propName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<ResultModel> SetEventAttributeAsync(Guid? eventId, string propName, string value)
        {
            var response = new ResultModel();
            if (!eventId.HasValue || propName.IsNullOrEmpty())
            {
                response.Errors.Add(new ErrorModel(string.Empty, "Invalid parameters"));
                return response;
            }

            await _context.Attributes.AddAsync(new EventAttribute
            {
                AttributeName = propName,
                EventId = eventId.Value,
                Value = value
            });

            return await _context.PushAsync();
        }

        /// <inheritdoc />
        /// <summary>
        /// Get event attribute value
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public async Task<ResultModel<string>> GetEventAttributeAsync(Guid? eventId, string propName)
        {
            var response = new ResultModel<string>();
            if (!eventId.HasValue || propName.IsNullOrEmpty())
            {
                response.Errors.Add(new ErrorModel(string.Empty, "Invalid parameters"));
                return response;
            }

            var dbResult =
                await _context.Attributes.FirstOrDefaultAsync(x =>
                    x.EventId.Equals(eventId) && x.AttributeName.Equals(propName));
            if (dbResult == null)
            {
                response.Errors.Add(new ErrorModel(string.Empty, "Attribute not found"));
                return response;
            }

            response.IsSuccess = true;
            response.Result = dbResult.Value;
            return response;
        }
    }
}
