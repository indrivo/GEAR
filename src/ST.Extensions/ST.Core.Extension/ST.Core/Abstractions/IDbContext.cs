using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ST.Core.Abstractions
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
    }
}
