using System;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using ST.Calendar.Abstractions;
using ST.Calendar.Abstractions.ExternalProviders;
using ST.Calendar.Abstractions.Models.ViewModels;
using ST.Calendar.Providers.Outlook.Helpers;
using ST.Calendar.Providers.Outlook.Mappers;
using ST.Core.Helpers;

namespace ST.Calendar.Providers.Outlook
{
    public class OutlookCalendarProvider : IExternalCalendarProvider
    {
        /// <summary>
        /// Graph client
        /// </summary>
        private GraphServiceClient _graphClient;

        /// <summary>
        /// Auth settings
        /// </summary>
        private readonly MsAuthorizationSettings _authSettings;

        /// <summary>
        /// Is authorized state
        /// </summary>
        private bool _isAuthorized;

        #region Injectable
        /// <summary>
        /// Inject token provider
        /// </summary>
        private readonly ICalendarExternalTokenProvider _tokenProvider;

        /// <summary>
        /// Inject settings service
        /// </summary>
        private readonly ICalendarUserSettingsService _settingsService;
        #endregion

        public OutlookCalendarProvider(ICalendarExternalTokenProvider tokenProvider, ICalendarUserSettingsService settingsService)
        {
            _tokenProvider = tokenProvider;
            _settingsService = settingsService;
            _authSettings = OutlookAuthSettings.GetAuthSettings();
            if (_authSettings == null) throw new Exception("No client settings present");
        }

        /// <inheritdoc />
        /// <summary>
        /// Authorize user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="reset"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> AuthorizeAsync(Guid? userId, bool reset = false)
        {
            var response = new ResultModel();
            try
            {
                _graphClient = new GraphServiceClient(GetAuthProvider(userId));
                var me = await _graphClient.Me.Request().GetAsync();
                if (me != null)
                {
                    _isAuthorized = true;
                    response.IsSuccess = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return response;
        }

        /// <inheritdoc />
        /// <summary>
        /// Push event to outlook calendar
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> PushEventAsync(GetEventViewModel evt)
        {
            var response = new ResultModel();
            if (!_isAuthorized) return response;
            try
            {
                var requestResult = await _graphClient.Me.Events
                    .Request()
                    .Header("Prefer", "outlook.timezone=\"Pacific Standard Time\"")
                    .AddAsync(OutlookMapper.Map(evt));

                await _settingsService.SetEventAttributeAsync(evt.Id, $"{nameof(OutlookCalendarProvider)}_evtId", requestResult.Id);
                response.IsSuccess = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return response;
        }

        /// <inheritdoc />
        /// <summary>
        /// Update event on provider
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="evtId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> UpdateEventAsync(GetEventViewModel evt, string evtId)
        {
            var response = new ResultModel();
            if (evtId.IsNullOrEmpty() || !_isAuthorized)
            {
                response.Errors.Add(new ErrorModel(string.Empty, "Invalid parameters"));
                return response;
            }

            try
            {
                var outLookEvt = await _graphClient.Me.Events[evtId]
                    .Request()
                    .Header("Prefer", "outlook.timezone=\"Pacific Standard Time\"")
                    .Select(e => new
                    {
                        e.Subject,
                        e.Body,
                        e.BodyPreview,
                        e.Organizer,
                        e.Attendees,
                        e.Start,
                        e.End,
                        e.Location
                    })
                    .GetAsync();
                if (evt == null) throw new Exception("Event not found");

                await _graphClient.Me.Events[evtId]
                    .Request()
                    .UpdateAsync(OutlookMapper.Map(outLookEvt, evt));

                response.IsSuccess = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return response;
        }

        /// <inheritdoc />
        /// <summary>
        /// Delete event by id
        /// </summary>
        /// <param name="evtId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> DeleteEventAsync(string evtId)
        {
            var response = new ResultModel();
            if (!_isAuthorized) return response;
            try
            {
                await _graphClient.Me.Events[evtId]
                    .Request()
                    .DeleteAsync();
                response.IsSuccess = true;
            }
            catch (Exception e)
            {
                response.Errors.Add(new ErrorModel(string.Empty, e.Message));
            }

            return response;
        }

        /// <summary>
        /// Get auth provider
        /// </summary>
        /// <returns></returns>
        private MsOutlookAuthenticationProvider GetAuthProvider(Guid? userId)
        {
            var cca = ConfidentialClientApplicationBuilder.Create(_authSettings.ClientId)
                .WithAuthority(_authSettings.GetAuthority())
                .WithRedirectUri("http://localhost:9099/ExternalLoginCallback")
                .WithClientSecret(_authSettings.ClientSecretId)
                .Build();

            return new MsOutlookAuthenticationProvider(cca, userId);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
