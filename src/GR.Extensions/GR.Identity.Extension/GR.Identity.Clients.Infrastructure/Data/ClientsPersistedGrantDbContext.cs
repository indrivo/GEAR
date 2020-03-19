using GR.Identity.Clients.Abstractions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace GR.Identity.Clients.Infrastructure.Data
{
    public class ClientsPersistedGrantDbContext : PersistedGrantDbContext<ClientsPersistedGrantDbContext>, IClientsPersistedGrantContext
    {
        public ClientsPersistedGrantDbContext(DbContextOptions<ClientsPersistedGrantDbContext> options, OperationalStoreOptions storeOptions) : base(options, storeOptions)
        {
        }
    }
}
