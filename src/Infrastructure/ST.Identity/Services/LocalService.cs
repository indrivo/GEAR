using IdentityServer4.EntityFramework.DbContexts;
using ST.Identity.Abstractions;
using System.Linq;

namespace ST.Identity.Services
{
    public class LocalService : ILocalService
    {
        private readonly ConfigurationDbContext _context;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public LocalService(ConfigurationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get application Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetAppName(string name)
        {
            var result = _context.Clients.FirstOrDefault(x => x.ClientId.Equals(name))?.ClientName;

            return result;
        }
    }
}
