using System;
using System.Threading;
using System.Threading.Tasks;
using GR.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace GR.Core.Abstractions
{
    public interface IDbContext
    {
        /// <summary>
        /// Set entity for work
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        DbSet<T> SetEntity<T>() where T : class, IBaseModel;

        /// <summary>
        /// Update db entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        EntityEntry<TEntity> Update<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// Save changes on database
        /// </summary>
        /// <returns></returns>
        int SaveChanges();

        /// <summary>
        /// Save changes async
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Seed data
        /// </summary>
        /// <returns></returns>
        Task InvokeSeedAsync(IServiceProvider services);

        /// <summary>
        /// Remove by id
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TIdType"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResultModel> RemoveByIdAsync<TEntity, TIdType>(TIdType id)
            where TEntity : class, IBaseModel, IBase<TIdType>;

        /// <summary>
        /// Find by id
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TIdType"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResultModel<TEntity>> FindByIdAsync<TEntity, TIdType>(TIdType id)
            where TEntity : class, IBaseModel, IBase<TIdType>;
    }
}
