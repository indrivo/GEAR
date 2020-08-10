using System;
using System.Threading.Tasks;
using GR.Core.Abstractions;
using GR.Core.Attributes;
using GR.Core.Helpers;
using GR.Identity.Clients.Abstractions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace GR.Identity.Clients.Infrastructure.Data
{
    [IgnoreContextAutoMigrations]
    public class ClientsConfigurationDbContext : ConfigurationDbContext<ClientsConfigurationDbContext>, IClientsContext
    {
        public ClientsConfigurationDbContext(DbContextOptions<ClientsConfigurationDbContext> options, ConfigurationStoreOptions storeOptions) : base(options, storeOptions)
        {
        }

        public Task InvokeSeedAsync(IServiceProvider services)
        {
            return Task.CompletedTask;
        }

        public Task<ResultModel> RemoveByIdAsync<TEntity, TIdType>(TIdType id) where TEntity : class, IBaseModel, IBase<TIdType>
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel<TEntity>> FindByIdAsync<TEntity, TIdType>(TIdType id) where TEntity : class, IBaseModel, IBase<TIdType>
        {
            throw new NotImplementedException();
        }
    }
}