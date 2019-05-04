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

        /// <summary>
        /// Set new app name
        /// </summary>
        /// <param name="app"></param>
        /// <param name="name"></param>
        public void SetAppName(string app, string name)
        {
            var appData = _context.Clients.FirstOrDefault(x => x.ClientId.Equals(name));
            if (appData != null)
            {
                try
                {
                    appData.ClientName = name;
                    _context.Clients.Update(appData);
                    _context.SaveChanges();
                }
                catch
                {
                    //Ignore
                }
            }
        } 
    }
}
