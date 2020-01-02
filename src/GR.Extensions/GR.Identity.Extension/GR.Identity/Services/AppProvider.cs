using GR.Identity.Abstractions;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GR.Identity.Services
{
    public class AppProvider : IAppProvider
    {
        private readonly ConfigurationDbContext _configurationDbContext;

        public AppProvider(ConfigurationDbContext configurationDbContext)
        {
            _configurationDbContext = configurationDbContext;
        }

        /// <summary>
        /// Get app name
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public async Task<string> GetAppName(string appId)
        {
            var appName = await _configurationDbContext.Clients.FirstOrDefaultAsync(x => x.ClientId.Equals("core"));
            return appName?.ClientName ?? "No app found";
        }
    }
}