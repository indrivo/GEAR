using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using GR.Calendar.Abstractions;
using GR.Core.Helpers;

namespace GR.Calendar.Providers.Outlook
{
    public class MsOutlookAuthenticationProvider : IAuthenticationProvider
    {
        /// <summary>
        /// Client
        /// </summary>
        private readonly IConfidentialClientApplication _clientApplication;

        /// <summary>
        /// User id
        /// </summary>
        private readonly Guid? _userId;

        /// <summary>
        /// User tokens
        /// </summary>
        private IEnumerable<AuthenticationToken> _tokens = new List<AuthenticationToken>();

        /// <summary>
        /// Inject token provider
        /// </summary>
        private readonly ICalendarExternalTokenProvider _externalTokenProvider;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clientApplication"></param>
        /// <param name="userId"></param>
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
            //TODO: Check if token is expired
            if (_tokens.Any())
            {
                return _tokens.FirstOrDefault(x => x.Name.Equals("access_token"))?.Value;
            }

            var dbTokens = await _externalTokenProvider.GetTokenAsync<IEnumerable<AuthenticationToken>>(nameof(OutlookCalendarProvider), _userId);
            if (!dbTokens.IsSuccess) return null;
            var tokenRequest = dbTokens.Result.FirstOrDefault(x => x.Name.Equals("access_token"));
            if (dbTokens.Result.Any()) _tokens = dbTokens.Result;
            return tokenRequest?.Value;
        }
    }
}
