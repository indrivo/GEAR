using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using ST.Calendar.Abstractions;
using ST.Calendar.Abstractions.ExternalProviders;
using ST.Calendar.Abstractions.Models;
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
        #endregion

        public GoogleCalendarProvider(ICalendarExternalTokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
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

            var tokenRequest = await _tokenProvider.GetTokenAsync<UserCredential>(nameof(GoogleCalendarProvider), userId);

            if (tokenRequest.IsSuccess)
            {
                _credential = tokenRequest.Result;
                if (_credential.Token.IsExpired(_credential.Flow.Clock))
                {
                    var refreshState = await _credential.RefreshTokenAsync(CancellationToken.None);
                    if (refreshState) await _tokenProvider.SetTokenAsync(new ExternalProviderToken
                    {
                        UserId = userId.GetValueOrDefault(),
                        ProviderName = nameof(GoogleCalendarProvider),
                        Value = _credential.Serialize()
                    });
                }
            }
            else
            {
                using (var stream = new FileStream(Path.Combine(AppContext.BaseDirectory, "googleCredentials.json"), FileMode.Open, FileAccess.Read))
                {
                    _credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        userId.ToString(),
                        CancellationToken.None);

                    await _tokenProvider.SetTokenAsync(new ExternalProviderToken
                    {
                        UserId = userId.GetValueOrDefault(),
                        ProviderName = nameof(GoogleCalendarProvider),
                        Value = _credential.Serialize()
                    });
                }
            }

            InitService();
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

        public void Test()
        {
            var request = _service.Events.List("primary");
            request.TimeMin = DateTime.MinValue;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            var events = request.Execute();
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
            var googleEvent = GoogleCalendarMapper.Map(evt);
            var request = _service.Events.Insert(googleEvent, CalendarName);

            try
            {
                await request.ExecuteAsync();
                response.IsSuccess = true;
            }
            catch (Exception e)
            {
                response.Errors.Add(new ErrorModel(e.Message));
            }
            return response;
        }

        public void Dispose()
        {
            _service?.Dispose();
        }
    }
}
