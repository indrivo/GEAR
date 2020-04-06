using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Services;
using Microsoft.EntityFrameworkCore;

namespace GR.Identity.Clients.Abstractions.Helpers
{
    public class CustomCorsPolicyService : ICorsPolicyService
    {
        #region Injectable

        /// <summary>
        /// Inject clients context
        /// </summary>
        private readonly IClientsContext _clientsContext;

        #endregion

        public CustomCorsPolicyService(IClientsContext clientsContext)
        {
            _clientsContext = clientsContext;
        }

        /// <summary>
        /// Check if is allowed origin
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public virtual async Task<bool> IsOriginAllowedAsync(string origin)
        {
            var clients = await _clientsContext.Clients
                .Include(x => x.AllowedCorsOrigins)
                .ToListAsync();
            var result = clients.Select(x => x.ToModel()).SelectMany(client => client.AllowedCorsOrigins as IEnumerable<string>, (client, url) => GetOrigin(url)).Contains(origin, StringComparer.OrdinalIgnoreCase);
            return result;
        }

        /// <summary>
        /// Get origin
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetOrigin(string url)
        {
            if (url == null) return null;
            Uri uri;
            try
            {
                uri = new Uri(url);
            }
            catch (Exception)
            {
                return null;
            }
            if (uri.Scheme == "http" || uri.Scheme == "https")
                return uri.Scheme + "://" + uri.Authority;
            return null;
        }
    }
}
