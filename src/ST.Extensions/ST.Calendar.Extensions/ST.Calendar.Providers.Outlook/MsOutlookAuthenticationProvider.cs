using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using ST.Core.Helpers;
using ST.Identity.Abstractions;

namespace ST.Calendar.Providers.Outlook
{
    public class MsOutlookAuthenticationProvider : IAuthenticationProvider
    {
        private readonly IConfidentialClientApplication _clientApplication;
        public MsOutlookAuthenticationProvider(IConfidentialClientApplication clientApplication)
        {
            _clientApplication = clientApplication;
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
            var service = IoC.Resolve<SignInManager<ApplicationUser>>();
            var info = await service.GetExternalLoginInfoAsync();
            var tokenRequest = info?.AuthenticationTokens.FirstOrDefault(x => x.Name.Equals("access_token"));
            return tokenRequest?.Value;
        }
    }
}
