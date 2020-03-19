using GR.Core.Helpers.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore.Design;

namespace GR.Identity.Clients.Infrastructure.Data
{
    public class PersistedGrantDbContextFactory : IDesignTimeDbContextFactory<ClientsPersistedGrantDbContext>
    {
        /// <inheritdoc />
        /// <summary>
        /// For creating migrations
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public ClientsPersistedGrantDbContext CreateDbContext(string[] args)
        {
            var storeOptions = new OperationalStoreOptions
            {
                DefaultSchema = "Identity"
            };

            return DbContextFactory<ClientsPersistedGrantDbContext, ClientsPersistedGrantDbContext>
                .CreateFactoryDbContext(storeOptions);
        }
    }
}