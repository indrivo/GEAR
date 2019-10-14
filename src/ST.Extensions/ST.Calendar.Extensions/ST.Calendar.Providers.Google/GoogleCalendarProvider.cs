using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using ST.Calendar.Abstractions;
using ST.Calendar.Abstractions.ExternalProviders;
using ST.Calendar.Abstractions.Models.ViewModels;
using ST.Calendar.Providers.Google.Mappers;
using ST.Core.Extensions;
using ST.Core.Helpers;
using Google.Apis.People.v1;
using Microsoft.EntityFrameworkCore.Internal;
using ST.Calendar.Abstractions.ExternalProviders.Helpers;

namespace ST.Calendar.Providers.Google
{
    public sealed class GoogleCalendarProvider : IExternalCalendarProvider
    {
        /// <summary>
        /// Calendar service
        /// </summary>
        private CalendarService _calendarService;

        /// <summary>
        /// People service
        /// </summary>
        private PeopleService _peopleService;

        /// <summary>
        /// User credentials
        /// </summary>
        private UserCredential _credential;

        /// <summary>
        /// Provider scopes
        /// </summary>
        private static readonly string[] Scopes = {
            CalendarService.Scope.CalendarReadonly,
            CalendarService.Scope.Calendar,
            PeopleService.Scope.UserinfoProfile,
            PeopleService.Scope.ContactsReadonly,
            PeopleService.Scope.Contacts,
            PeopleService.Scope.UserAddressesRead,
            PeopleService.Scope.UserEmailsRead,
        };

        /// <summary>
        /// Calendar name
        /// </summary>
        private const string CalendarName = "primary";

        /// <summary>
        /// Set or get state of session
        /// </summary>
        private bool Authorized { get; set; }

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

        public GoogleCalendarProvider(ICalendarExternalTokenProvider tokenProvider, ICalendarUserSettingsService settingsService)
        {
            _tokenProvider = tokenProvider;
            _settingsService = settingsService;
        }

        /// <inheritdoc />
        /// <summary>
        /// Authorize
        /// </summary>
        /// <returns></returns>
        public async Task<ResultModel> AuthorizeAsync(Guid? userId, bool reset = false)
        {
            var response = new ResultModel();
            var shouldRevokeToken = false;
            try
            {
                using (var stream = new FileStream(Path.Combine(AppContext.BaseDirectory, "googleCredentials.json"), FileMode.Open, FileAccess.Read))
                {
                    if (reset)
                    {
                        var dbToken = await _tokenProvider.GetTokenAsync<TokenResponse>(nameof(GoogleCalendarProvider), userId);
                        shouldRevokeToken = dbToken.IsSuccess;
                    }

                    _credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        userId.ToString(),
                        CancellationToken.None, new GoogleDataStore(_tokenProvider));

                    if (_credential.Token.IsExpired(_credential.Flow.Clock))
                    {
                        await _credential.RefreshTokenAsync(CancellationToken.None);
                    }

                    if (shouldRevokeToken)
                    {
                        var revokeResponse = await _credential.RevokeTokenAsync(CancellationToken.None);
                        if (revokeResponse) return await AuthorizeAsync(userId);
                    }

                    response.IsSuccess = true;
                }

                InitServices();
                Authorized = true;
            }
            catch (Exception e)
            {
                response.Errors.Add(new ErrorModel(string.Empty, e.Message));
            }

            return response;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get user 
        /// </summary>
        /// <returns></returns>
        public async Task<ResultModel<ExternalCalendarUser>> GetUserAsync()
        {
            var response = new ResultModel<ExternalCalendarUser>();
            if (!Authorized) return response;
            try
            {
                var userRequest = _peopleService.People.Get("people/me");

                userRequest.RequestMaskIncludeField = new List<string>
                {
                    "person.emailAddresses",
                    "person.names",
                    "person.photos"
                }.Join(",");

                var user = await userRequest.ExecuteAsync();

                response.Result = new ExternalCalendarUser
                {
                    DisplayName = user.Names?.FirstOrDefault()?.DisplayName,
                    EmailAddress = user.EmailAddresses?.FirstOrDefault()?.Value,
                    ImageUrl = user.Photos?.FirstOrDefault()?.Url
                };
                response.IsSuccess = true;
            }
            catch (Exception e)
            {
                response.Errors.Add(new ErrorModel(string.Empty, e.Message));
            }

            return response;
        }

        /// <summary>
        /// Init service
        /// </summary>
        private void InitServices()
        {
            var initializer = new BaseClientService.Initializer
            {
                HttpClientInitializer = _credential,
                ApplicationName = CalendarName,
            };

            _calendarService = new CalendarService(initializer);

            _peopleService = new PeopleService(initializer);
        }

        /// <inheritdoc />
        /// <summary>
        /// Update event
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="evtId"></param>
        /// <returns></returns>
        public async Task<ResultModel> UpdateEventAsync(GetEventViewModel evt, string evtId)
        {
            var response = new ResultModel();
            if (!Authorized || evtId.IsNullOrEmpty()) return response;
            try
            {
                var request = _calendarService.Events.Get(CalendarName, evtId);
                var googleEvent = await request.ExecuteAsync();

                var updateRequest = _calendarService.Events.Update(GoogleCalendarMapper.Map(googleEvent, evt), CalendarName, evtId);

                await updateRequest.ExecuteAsync();
                response.IsSuccess = true;
            }
            catch (Exception e)
            {
                response.Errors.Add(new ErrorModel(string.Empty, e.Message));
            }

            return response;
        }


        /// <inheritdoc />
        /// <summary>
        /// Delete provider event
        /// </summary>
        /// <param name="evtId"></param>
        /// <returns></returns>
        public async Task<ResultModel> DeleteEventAsync(string evtId)
        {
            var response = new ResultModel();
            if (!Authorized || evtId.IsNullOrEmpty()) return response;
            try
            {
                var deleteRequest = _calendarService.Events.Delete(CalendarName, evtId);
                await deleteRequest.ExecuteAsync();
            }
            catch (Exception e)
            {
                response.Errors.Add(new ErrorModel(string.Empty, e.Message));
            }

            return response;
        }

        /// <inheritdoc />
        /// <summary>
        /// Create new event
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        public async Task<ResultModel> PushEventAsync(GetEventViewModel evt)
        {
            var response = new ResultModel();
            if (!Authorized) return response;
            var googleEvent = GoogleCalendarMapper.Map(evt);

            try
            {
                var request = _calendarService.Events.Insert(googleEvent, CalendarName);
                var requestResult = await request.ExecuteAsync();
                response.IsSuccess = true;
                response.Result = requestResult.Id;
                await _settingsService.SetEventAttributeAsync(evt.Id, $"{nameof(GoogleCalendarProvider)}_evtId", requestResult.Id);
            }
            catch (Exception e)
            {
                response.Errors.Add(new ErrorModel(e.Message));
            }
            return response;
        }

        /// <inheritdoc />
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            _calendarService?.Dispose();
        }
    }
}
