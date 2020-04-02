using GR.Core.Helpers.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore.Design;

namespace GR.Identity.Clients.Infrastructure.Data
{
    public class ConfigurationDbContextFactory : IDesignTimeDbContextFactory<ClientsConfigurationDbContext>
    {
        /// <inheritdoc />
        /// <summary>
        /// For creating migrations
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public ClientsConfigurationDbContext CreateDbContext(string[] args)
        {
            var storeOptions = new ConfigurationStoreOptions
            {
                DefaultSchema = "Identity"
            };

            return DbContextFactory<ClientsConfigurationDbContext, ClientsConfigurationDbContext>
                .CreateFactoryDbContext(storeOptions);
        }
    }
}