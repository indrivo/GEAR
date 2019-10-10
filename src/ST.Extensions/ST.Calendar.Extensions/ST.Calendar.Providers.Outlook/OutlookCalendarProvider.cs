using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
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
        #region Injectable


        private readonly GraphServiceClient _graphClient;

        /// <summary>
        /// Auth settings
        /// </summary>
        private readonly MsAuthorizationSettings _authSettings;

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

        public virtual async Task<ResultModel> AuthorizeAsync(Guid? userId)
        {
            var response = new ResultModel();

            var graph = await GetGraphServiceClient();



            return response;
        }

        public virtual async Task<ResultModel> PushEventAsync(GetEventViewModel evt)
        {
            var response = new ResultModel();


            try
            {
                var request = await _graphClient.Me.Events
                    .Request()
                    .Header("Prefer", "outlook.timezone=\"Pacific Standard Time\"")
                    .AddAsync(OutlookMapper.Map(evt));

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return response;
        }

        public Task<ResultModel> UpdateEventAsync(GetEventViewModel evt, string evtId)
        {
            throw new NotImplementedException();
        }

        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task<GraphServiceClient> GetGraphServiceClient()
        {
            var authProvider = await GetAuthProvider();
            return new GraphServiceClient(authProvider);
        }

        private async Task<MsOutlookAuthenticationProvider> GetAuthProvider()
        {
            var cca = ConfidentialClientApplicationBuilder.Create(_authSettings.ClientId)
                .WithAuthority(_authSettings.GetAuthority())
                .WithRedirectUri("http://localhost:9099/ExternalLoginCallback")
                .WithClientSecret(_authSettings.ClientSecretId)
                .Build();

            return new MsOutlookAuthenticationProvider(cca);
        }
    }
}
