using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using ST.Calendar.Abstractions;
using ST.Calendar.Abstractions.ExternalProviders;
using ST.Calendar.Abstractions.Models.ViewModels;
using ST.Calendar.Providers.Google.Mappers;
using ST.Core.Extensions;
using ST.Core.Helpers;

namespace ST.Calendar.Providers.Google
{
    public sealed class GoogleCalendarProvider : IExternalCalendarProvider
    {
        private CalendarService _service;
        private static readonly string[] Scopes = { CalendarService.Scope.CalendarReadonly, CalendarService.Scope.Calendar };
        private const string CalendarName = "primary";
        private UserCredential _credential;

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
        public async Task<ResultModel> AuthorizeAsync(Guid? userId)
        {
            var response = new ResultModel();
            if (Authorized && _credential.UserId.Equals(userId.ToString()))
            {
                response.IsSuccess = true;
                return response;
            }

            try
            {
                using (var stream = new FileStream(Path.Combine(AppContext.BaseDirectory, "googleCredentials.json"), FileMode.Open, FileAccess.Read))
                {
                    _credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        userId.ToString(),
                        CancellationToken.None, new GoogleDataStore(_tokenProvider));

                    response.IsSuccess = true;
                }

                InitService();
                Authorized = true;
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
        private void InitService()
        {
            _service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = _credential,
                ApplicationName = CalendarName,
            });
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
                var request = _service.Events.Get(CalendarName, evtId);
                var googleEvent = await request.ExecuteAsync();

                var updateRequest = _service.Events.Update(GoogleCalendarMapper.Map(googleEvent, evt), CalendarName, evtId);

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
                var deleteRequest = _service.Events.Delete(CalendarName, evtId);
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
                var request = _service.Events.Insert(googleEvent, CalendarName);
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
            _service?.Dispose();
        }
    }
}
