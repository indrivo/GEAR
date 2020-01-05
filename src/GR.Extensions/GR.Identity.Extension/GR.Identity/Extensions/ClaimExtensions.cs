using IdentityServer4.EntityFramework.DbContexts;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace GR.Identity.Extensions
{
    public static class ClaimExtensions
    {
        public static IList<Claim> GetSystemClaims(this IList<Claim> claims, ConfigurationDbContext context)
        {
            var sysClaims = new List<Claim>();
            var clients = context.Clients.Select(x => x.ClientId).ToList();
            foreach (var client in clients)
            {
                sysClaims.AddRange(claims.Where(claim => claim.Type.ToLower().StartsWith(client)));
            }
            return sysClaims;
        }
    }
}