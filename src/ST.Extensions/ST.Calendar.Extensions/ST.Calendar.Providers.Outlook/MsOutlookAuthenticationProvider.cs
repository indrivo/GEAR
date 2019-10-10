using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using ST.Calendar.Abstractions;
using ST.Core.Helpers;

namespace ST.Calendar.Providers.Outlook
{
    public class MsOutlookAuthenticationProvider : IAuthenticationProvider
    {
        private readonly IConfidentialClientApplication _clientApplication;
        private readonly Guid? _userId;
        /// <summary>
        /// Inject token provider
        /// </summary>
        private readonly ICalendarExternalTokenProvider _externalTokenProvider;

        public MsOutlookAuthenticationProvider(IConfidentialClientApplication clientApplication, Guid? userId)
        {
            _clientApplication = clientApplication;
            _userId = userId;
            _externalTokenProvider = IoC.Resolve<ICalendarExternalTokenProvider>();
        }

        /// <inheritdoc />
        /// <summary>
        /// Authenticate request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            var token = await GetTokenAsync();
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
        }

        /// <summary>
        /// Get token
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetTokenAsync()
        {
            var dbTokens = await _externalTokenProvider.GetTokenAsync<IEnumerable<AuthenticationToken>>(nameof(OutlookCalendarProvider), _userId);
            if (!dbTokens.IsSuccess) return null;
            var tokenRequest = dbTokens.Result.FirstOrDefault(x => x.Name.Equals("access_token"));
            return tokenRequest?.Value;
        }
    }
}
