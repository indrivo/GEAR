using System;
using System.Threading.Tasks;
using GR.Core.Abstractions;
using GR.Core.Attributes;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Identity.Clients.Abstractions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace GR.Identity.Clients.Infrastructure.Data
{
    [Author(Authors.LUPEI_NICOLAE)]
    [IgnoreContextAutoMigrations]
    public class ClientsPersistedGrantDbContext : PersistedGrantDbContext<ClientsPersistedGrantDbContext>, IClientsPersistedGrantContext
    {
        public ClientsPersistedGrantDbContext(DbContextOptions<ClientsPersistedGrantDbContext> options, OperationalStoreOptions storeOptions) : base(options, storeOptions)
        {
        }

        public Task<ResultModel> RemoveByIdAsync<TEntity, TIdType>(TIdType id) where TEntity : class, IBaseModel, IBase<TIdType>
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel<TEntity>> FindByIdAsync<TEntity, TIdType>(TIdType id) where TEntity : class, IBaseModel, IBase<TIdType>
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Invoke seed
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public virtual Task InvokeSeedAsync(IServiceProvider services)
        {
            return Task.CompletedTask;
        }
    }
}
