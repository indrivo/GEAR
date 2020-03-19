using GR.Identity.Clients.Abstractions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace GR.Identity.Clients.Infrastructure.Data
{
    public class ClientsConfigurationDbContext : ConfigurationDbContext<ClientsConfigurationDbContext>, IClientsContext
    {
        public ClientsConfigurationDbContext(DbContextOptions<ClientsConfigurationDbContext> options, ConfigurationStoreOptions storeOptions) : base(options, storeOptions)
        {
        }
    }
}
